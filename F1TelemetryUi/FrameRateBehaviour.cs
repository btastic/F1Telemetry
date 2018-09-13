using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace F1TelemetryUi
{
    public sealed class FrameRateBehavior : Behavior<TextBlock>
    {
        private readonly Queue<long> _ticks;

        public FrameRateBehavior()
        {
            _ticks = new Queue<long>();
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            CompositionTarget.Rendering += CalculateFrameRate;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            CompositionTarget.Rendering -= CalculateFrameRate;
            _ticks.Clear();
        }

        private void CalculateFrameRate(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;

            while (_ticks.Any())
            {
                var startTime = now.AddSeconds(-1).Ticks;
                if (_ticks.Peek() < startTime)
                {
                    _ticks.Dequeue();

                    continue;
                }

                break;
            }

            var endTime = now.Ticks;
            _ticks.Enqueue(endTime);
            var count = _ticks.Count;

            AssociatedObject.Text = "FPS: " + count;
        }
    }
}
