using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mandara.ProductGUI.Desks
{
    interface IDesksRepository
    {
        bool LoadDesks();
        bool CanAccessDesks();
        DatabaseActionResult Add(Desk newDesk);
        DatabaseActionResult Update(Desk changedDesk);
        DatabaseActionResult Remove(Desk deskToDelete);
        List<Desk> Desks { get; }
        ObservableCollection<Desk> DesksData { get; }
        Desk GetDesk(string name);
        int TotalDesks();
    }
}