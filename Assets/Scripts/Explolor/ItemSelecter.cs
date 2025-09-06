using CommonUtilLib.ThreadSafe;

class ItemSelecter : SingleTonForGameObject<ItemSelecter>
{
    static bool curSelectedIndex = false;
    private void Awake()
    {
        SetInstance(this);
    }

  


    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}
