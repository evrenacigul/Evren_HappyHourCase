public interface ISelectable
{
    bool isSelected { get; }
    void Select();
    void Deselect();
}