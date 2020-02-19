namespace ILGenerator.BaseClasses
{
    public abstract class NotifyPropertyChangedBaseWithChangeTracker : NotifyPropertyChangedBase
    {
        public ChangeTracker ChangeTracker { get; private set; }
        public NotifyPropertyChangedBaseWithChangeTracker()
        {
            ChangeTracker = new ChangeTracker(this);
        }

        public override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnPropertyChanged(propertyName, oldValue, newValue);
            ChangeTracker?.OnChanged(propertyName, oldValue, newValue);
        }
    }
}
