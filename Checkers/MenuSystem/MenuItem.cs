using System;

namespace MenuSystem
{
    public class MenuItem
    {
        public virtual string Label { get; set; }

        public virtual Func<Action>? MethodToExecute { get; set; }

        public MenuItem(string label, Func<Action>? methodToExecute)
        {
            Label = label.Trim();
            MethodToExecute = methodToExecute;
        }

        public override string ToString()
        {
            return Label;
        }
    }
}