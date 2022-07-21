class Switcher
{
    private bool _enable;

    public void SwitchOn(bool enable)
    {
        _enable = enable;

        if (enable)
            _effects.StartEnableAnimation();
        else
            SwitchOff(enable);
    }

    public void SwitchOff(bool enable)
    {
        _enable = enable;

        if (!enable)
            _pool.Free(this);
        else
            SwitchOn(enable);
    }
}
