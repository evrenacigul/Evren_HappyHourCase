public interface IHarvestable
{
    bool isCollected { get; }

    float Collect();
}