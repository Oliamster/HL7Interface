using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NETF.Tests
{
    [TestFixture]
    public class Tasks
    {
        [Test]
        public async Task TestCancellation()
        {
            var m_ConnectionCancellationToken = new CancellationTokenSource();


            bool ret = false;
            int timeout = 100000;

            m_ConnectionCancellationToken.Cancel();

            try
            {
                var m_ConnectionTask = await Task.Factory.StartNew(async () =>
                {
                    while (!ret)
                    {
                        m_ConnectionCancellationToken.Token.ThrowIfCancellationRequested();

                        Thread.Sleep(100);
                    }
                }, m_ConnectionCancellationToken.Token);

                if (!m_ConnectionTask.Wait(timeout, m_ConnectionCancellationToken.Token))
                {
                    throw new Exception($"Connection timed out: {timeout}.");
                }
            }
            catch (OperationCanceledException)
            {
                //throw new OperationCanceledException("The connection task was cancelled.");
            }
            catch (Exception ex)
            {
            }

        }



        [Test]
        public async Task TestCancellation1()
        {
            var m_ConnectionCancellationToken = new CancellationTokenSource();


            bool ret = false;
            int timeout = 100000;

            m_ConnectionCancellationToken.Cancel();

            try
            {
                var m_ConnectionTask = await Task.Factory.StartNew(async () =>
                {
                    while (!ret)
                    {
                        //m_ConnectionCancellationToken.Token.ThrowIfCancellationRequested();

                        Thread.Sleep(100);
                    }
                }, m_ConnectionCancellationToken.Token);

                if (!m_ConnectionTask.Wait(timeout, m_ConnectionCancellationToken.Token))
                {
                    throw new Exception($"Connection timed out: {timeout}.");
                }
            }
            catch (OperationCanceledException)
            {
                //throw new OperationCanceledException("The connection task was cancelled.");
            }
            catch (Exception ex)
            {
            }

        }

        [Test]
        public async Task TestCancellation2()
        {
            var m_ConnectionCancellationToken = new CancellationTokenSource();


            bool ret = false;
            int timeout = 100000;

            m_ConnectionCancellationToken.Cancel();

            try
            {
                var m_ConnectionTask = await Task.Factory.StartNew(async () =>
                {
                    while (!ret)
                    {
                        m_ConnectionCancellationToken.Token.ThrowIfCancellationRequested();

                        Thread.Sleep(100);
                    }
                });

                if (!m_ConnectionTask.Wait(timeout, m_ConnectionCancellationToken.Token))
                {
                    throw new Exception($"Connection timed out: {timeout}.");
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("The connection task was cancelled.");
            }
            catch (Exception ex)
            {
            }

        }


        [Test]
        public async Task TestCancellation3()
        {
            var m_ConnectionCancellationToken = new CancellationTokenSource();


            bool ret = false;
            int timeout = 100000;

            m_ConnectionCancellationToken.Cancel();

            try
            {
                var m_ConnectionTask = await Task.Factory.StartNew(async () =>
                {
                    while (!ret)
                    {
                        await Task.Delay(100);
                    }
                });

                if (!m_ConnectionTask.Wait(timeout, m_ConnectionCancellationToken.Token))
                {
                    throw new Exception($"Connection timed out: {timeout}.");
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("The connection task was cancelled.");
            }
            catch (Exception ex)
            {
            }
        }

        [Test]
        public async Task TestCancellation4()
        {
            var m_ConnectionCancellationToken = new CancellationTokenSource();

            bool ret = false;

            int timeout = -1;

            try
            {
                var m_ConnectionTask = await Task.Factory.StartNew(async () =>
                {
                    while (!ret)
                    {
                        await Task.Delay(100);
                    }
                });

                m_ConnectionCancellationToken.CancelAfter(1000);

                if (!m_ConnectionTask.Wait(timeout, m_ConnectionCancellationToken.Token))
                {
                    throw new Exception($"Connection timed out: {timeout}.");
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("The connection task was cancelled.");
            }
            catch (Exception ex)
            {
            }
        }


        [Test]
        public async Task TestCancellation5()
        {
            var m_ConnectionCancellationToken = new CancellationTokenSource();

            bool ret = false;

            int timeout = -1;
          

            try
            {
                var m_ConnectionTask = await Task.Factory.StartNew(async () =>
                {
                    while (!ret && !m_ConnectionCancellationToken.Token.IsCancellationRequested)
                    {
                        await Task.Delay(100);
                    }
                });

                m_ConnectionCancellationToken.CancelAfter(3000);

                try
                {
                    m_ConnectionTask.Wait(timeout, m_ConnectionCancellationToken.Token);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            catch (OperationCanceledException ex)
            {
                throw new OperationCanceledException("The connection task was cancelled.");
            }
            catch (Exception ex)
            {
            }
        }
    }
}
