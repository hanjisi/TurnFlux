using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TurnFlux
{
    internal class PriorityScheduler
    {
        private readonly SemaphoreSlim _busLock = new SemaphoreSlim(1, 1);
        private readonly SimplePriorityQueue<TaskCompletionSource<IDisposable>> _queue = new SimplePriorityQueue<TaskCompletionSource<IDisposable>>();
        private readonly object _syncRoot = new object();
        private CancellationTokenSource _currentActiveCts;
        private int _processing;

        /// <summary>
        /// 请求执行权
        /// </summary>
        /// <param name="priority">数字越小优先级越高</param>
        /// <param name="urgent">是否立即抢占当前任务</param>
        public async ValueTask<IDisposable> AcquireAsync(int priority, bool urgent, CancellationToken cancellationToken)
        {
            // 创建 tcs 交付执行权
            var tcs = new TaskCompletionSource<IDisposable>(TaskCreationOptions.RunContinuationsAsynchronously);

            // 入队
            _queue.Enqueue(tcs, priority);

            // 如果立即抢占
            if (urgent)
            {
                //取消当前任务
                Interlocked.Exchange(ref _currentActiveCts, null)?.Cancel();
            }

            // 尝试触发队列处理
            TriggerProcess();

            try
            {
                cancellationToken.Register(() => throw new OperationCanceledException(cancellationToken));
                return await tcs.Task;
            }
            catch
            {
                // 如果外部取消或异常，确保 tcs 标记取消
                tcs.TrySetCanceled();
                throw;
            }
        }

        /// <summary>
        /// 触发队列处理
        /// </summary>
        private void TriggerProcess()
        {
            if (Interlocked.CompareExchange(ref _processing, 1, 0) == 0)
            {
                _ = ProcessQueueAsync();
            }
        }

        /// <summary>
        /// 异步处理队列
        /// </summary>
        /// <returns></returns>
        private async Task ProcessQueueAsync()
        {
            try
            {
                // 循环尝试获取锁并处理队列
                while (true)
                {
                    // 尝试获取总线锁
                    if (!await _busLock.WaitAsync(0)) return;

                    TaskCompletionSource<IDisposable> next = null;

                    // 出队
                    lock (_syncRoot)
                    {
                        if (!_queue.TryDequeue(out next))
                        {
                            // 队列空，释放总线锁
                            _busLock.Release();
                            return;
                        }
                    }

                    // 创建专属 CTS
                    var cts = new CancellationTokenSource();
                    Interlocked.Exchange(ref _currentActiveCts, cts);
                    var releaser = new Releaser(this, cts);

                    if (next.TrySetResult(releaser))
                    {
                        // 成功交付。注意：此时我们拿着锁退出了，锁由 Releaser.Dispose 释放
                        return;
                    }

                    // 如果任务已取消，清理 CTS 并继续
                    cts.Dispose();
                    Interlocked.CompareExchange(ref _currentActiveCts, null, cts);
                    _busLock.Release();
                }
            }
            finally
            {
                Interlocked.Exchange(ref _processing, 0);
                // 检查自旋：如果在退出瞬间又有新任务进来，确保不被漏掉
                lock (_syncRoot)
                {
                    if (_queue.Count > 0) TriggerProcess();
                }
            }
        }

        private sealed class Releaser : IDisposable
        {
            private readonly PriorityScheduler _parent;
            private readonly CancellationTokenSource _cts;
            private int _disposed;

            public CancellationToken Token => _cts.Token;

            public Releaser(PriorityScheduler parent, CancellationTokenSource cts)
            {
                _parent = parent;
                _cts = cts;
            }

            public void Dispose()
            {
                if (Interlocked.Exchange(ref _disposed, 1) == 1) return;

                _cts.Dispose();
                Interlocked.CompareExchange(ref _parent._currentActiveCts, null, _cts);
                _parent._busLock.Release();

                // 释放后触发队列处理
                _parent.TriggerProcess();
            }
        }
    }
}
