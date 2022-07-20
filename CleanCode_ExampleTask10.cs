class Weapon
{
    private const int _emptyClip = 0;
    private const int _brush = 1;

    private int _bullets;

    public bool CanShoot() => _bullets > _emptyClip;

    public void Shoot() => _bullets -= CanShoot() ? _brush : _emptyClip;
}