namespace Broken.Scripts.Interface
{
    public interface IWeapon : IEquipable
    {
        public void OnChange(IWeapon weapon);
    }
}
