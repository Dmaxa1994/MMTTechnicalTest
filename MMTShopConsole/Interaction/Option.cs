using System;

namespace MMTShopConsole.Interaction
{
    class Option
    {
        public string Name { get; }
        public Action SelectedAction { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Display text</param>
        /// <param name="selectedAction">Action to perform when user presses enter</param>
        /// <param name="success"></param>
        public Option(string name, Action selectedAction)
        {
            Name = name;
            SelectedAction = selectedAction;
        }
    }
}
