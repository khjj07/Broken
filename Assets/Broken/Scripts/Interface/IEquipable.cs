using Broken.Scripts.Ingame;

namespace Broken.Scripts.Interface
{
    public interface IEquipable
    {
        public abstract void OnEquip(Player player);
        public abstract void OnUnequip(Player player);
    }
}
