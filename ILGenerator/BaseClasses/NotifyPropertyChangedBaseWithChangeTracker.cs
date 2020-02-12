namespace ILGenerator.BaseClasses
{
    public abstract class NotifyPropertyChangedBaseWithChangeTracker : NotifyPropertyChangedBase
    {
        public ChangeTracker ChangeTracker { get; private set; }
        public NotifyPropertyChangedBaseWithChangeTracker()
        {
            ChangeTracker = new ChangeTracker();
        }

        public override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            ChangeTracker?.Changed(propertyName, oldValue, newValue);
            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }
    }
}
