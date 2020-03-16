using System.Collections.Generic;
using UnityEngine;

public static class EntityHelper
{

    /**
     * Quick method for entity players to drop an item on the ground at a certain position.
     */

    public static void DropItemOnGround(EntityAlive entity, ItemValue _itemValue, int count = 1)
    {
        entity.world.GetGameManager().ItemDropServer(new ItemStack(_itemValue, count), entity.GetPosition(), new Vector3(0.5f, 0f, 0.5f), entity.belongsPlayerId, 60f, false);
    }


    /**
     * Quick method to lookup an entity ID.
     */

    public static int GetEntityIdFor(string entityName)
    {
        foreach (KeyValuePair<int, EntityClass> keyValuePair in EntityClass.list.Dict)
        {
            if (keyValuePair.Value.entityClassName == entityName)
            {
                return keyValuePair.Key;
            }
        }
        return 0;
    }


    /**
     * Spawns a number of entities around a given position.
     */

    public static void SpawnEntitiesAroundPosition(string _entityGroupName, Vector3 _pos, int _minRange, int _maxRange, int count, Entity target = null)
    {
        Vector3 spawnPoint;
        int lastEntityId = 0;

        for (int i = 0; i < count; i += 1)
        {
            if (!GameManager.Instance.World.GetRandomSpawnPositionMinMaxToPosition(_pos, _minRange, _maxRange, 1, false, out spawnPoint))
            {
                continue;
            }

            int nextEntityId = EntityGroups.GetRandomFromGroup(_entityGroupName, ref lastEntityId);
            if (nextEntityId == 0)
            {
                continue;
            }

            Entity entity = EntityFactory.CreateEntity(nextEntityId, spawnPoint);

            if (entity == null)
            {
                continue;
            }

            entity.SetSpawnerSource(EnumSpawnerSource.Dynamic);
            GameManager.Instance.World.SpawnEntityInWorld(entity);
        }
    }

}