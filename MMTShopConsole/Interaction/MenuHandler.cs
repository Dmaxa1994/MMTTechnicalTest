using System;
using System.Collections.Generic;

namespace MMTShopConsole.Interaction
{
    class MenuHandler
    {
        public static MenuHandler menuInstance;
        public bool DeleteEntry { get; set; } = false;
        public bool UpdatedEntry { get; set; } = false;

        /// <summary>
        /// This is used to update the users selected item on the display
        /// The "highlighted" option is denoted by >
        /// </summary>
        /// <param name="options">Options to display to the user, only the name is used from the object here</param>
        /// <param name="selectedOption">The current option that is selected, this is decided by using an index for the list position</param>
        /// <param name="sectionTitle">Additional text to display to the user</param>
        private void WriteMenu(List<Option> options, Option selectedOption, string sectionTitle = "")
        {
            Console.Clear();

            if (!string.IsNullOrWhiteSpace(sectionTitle))
                Console.WriteLine(sectionTitle);

            foreach (Option option in options)
            {
                if (option == selectedOption)
                {
                    Console.Write("> ");
                }
                else
                {
                    Console.Write(" ");
                }

                Console.WriteLine(option.Name);
            }
        }

        /// <summary>
        /// works on a do while loop to watch for the readkey event
        /// Only tracks, up, down and enter
        /// </summary>
        /// <param name="options">List of options to present to the user, the associated action is performed when the user presses enter</param>
        /// <param name="sectionTitle">Additional text to display to the user</param>
        public void HandleUserInput(List<Option> options, string sectionTitle = "")
        {
            int selectedIndex = 0;
            ConsoleKeyInfo keyinfo;
            WriteMenu(options, options[selectedIndex], sectionTitle);
            do
            {
                keyinfo = Console.ReadKey();

                // Handle each key input (down arrow will write the menu again with a different selected item)
                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (selectedIndex + 1 < options.Count)
                    {
                        selectedIndex++;
                        WriteMenu(options, options[selectedIndex], sectionTitle);
                    }
                }
                if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (selectedIndex - 1 >= 0)
                    {
                        selectedIndex--;
                        WriteMenu(options, options[selectedIndex], sectionTitle);
                    }
                }
                // Handle different action for the option
                if (keyinfo.Key == ConsoleKey.Enter)
                {
                    if (options[selectedIndex].Name == "Exit" || options[selectedIndex].Name == "Back")
                    {
                        break;
                    }

                    options[selectedIndex].SelectedAction.Invoke();

                    //if the user used the delete api and it was successful, we to reflect that in the display
                    if (DeleteEntry)
                    {
                        DeleteEntry = false;
                        options.RemoveAt(selectedIndex);
                    }

                    //if the user updated an entry, we want to back out of the current layer so the user can reload the object
                    if(UpdatedEntry)
                    {
                        UpdatedEntry = false;
                        return;
                    }

                    selectedIndex = 0;
                    WriteMenu(options, options[selectedIndex], sectionTitle);
                }
            }
            while (true);
        }

    }

}
