using UnityEngine;

public interface IHarvester
{
    void Harvest(IHarvestable harvestable, Vector3 position);
}