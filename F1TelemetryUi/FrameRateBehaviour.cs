using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace F1TelemetryUi
{
    public sealed class FrameRateBehavior : Behavior<TextBlock>
    {
        private readonly Queue<long> _ticks = new Queue<long>();

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
            var now = DateTime.Now;
            var endTime = now.Ticks;
            var startTime = now.AddSeconds(-1).Ticks;

            while (_ticks.Any())
            {
                if (_ticks.Peek() < startTime)
                {
                    _ticks.Dequeue();

                    continue;
                }

                break;
            }

            _ticks.Enqueue(endTime);
            var count = _ticks.Count;

            AssociatedObject.Text = "FPS: " + count;
        }
    }
}
