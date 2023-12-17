using UnityEngine;

namespace Game.Projectiles
{
    public class ProjectileData
    {
        public float Range;
        public float Power;
        public Transform Parent;
        public LayerMask Layer;
        public ProjectileBehaviour Behaviour;
        public ProjectileModifier Modifier;

        public ProjectileData(
            float range,
            float power,
            Transform parent,
            LayerMask layer,
            ProjectileBehaviour behaviour,
            ProjectileModifier modifier)
        {
            Range = range;
            Power = power;
            Parent = parent;
            Layer = layer;
            Behaviour = behaviour;
            Modifier = modifier;
        }
    }

    public enum ProjectileType
    {
        Stone,
        Axe,
        Spear,
        Sword,
        Arrow,
        Gun
    }

    public enum ProjectileBehaviour
    {
        Standard,
        Ricochet,
        Boomerang
    }

    public enum ProjectileModifier
    {
        Standard,
        Fire,
        Ice
    }
}