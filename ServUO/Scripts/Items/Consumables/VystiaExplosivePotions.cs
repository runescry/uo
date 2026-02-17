#region References
using System;
using System.Collections.Generic;
using System.Linq;
using Server.Network;
using Server.Spells;
using Server.Targeting;
#endregion

namespace Server.Items
{
    /// <summary>
    /// Vystia Engineering Explosives - Creates mechanical sparks and smoke cloud
    /// Used by Engineering crafting system
    /// </summary>
    public class VystiaLesserEngineeringExplosive : BaseExplosionPotion
    {
        [Constructable]
        public VystiaLesserEngineeringExplosive()
            : base(PotionEffect.ExplosionLesser)
        {
            Hue = 2401; // Metallic/mechanical hue
            Name = "Small Mechanical Explosive";
        }

        public VystiaLesserEngineeringExplosive(Serial serial)
            : base(serial)
        { }

        public override int MinDamage { get { return 5; } }
        public override int MaxDamage { get { return 10; } }

        public override void Drink(Mobile from)
        {
            if (Core.AOS && (from.Paralyzed || from.Frozen || (from.Spell != null && from.Spell.IsCasting)))
            {
                from.SendLocalizedMessage(1062725);
                return;
            }

            Stackable = false;
            from.RevealingAction();
            from.Target = new VystiaThrowTarget(this);

            from.SendLocalizedMessage(500236); // You should throw it now!

            Timer.DelayCall(
                TimeSpan.FromSeconds(1.0),
                TimeSpan.FromSeconds(1.25),
                5,
                new TimerStateCallback(VystiaDetonate_OnTick),
                new object[] { from, 3 });
        }

        private void VystiaDetonate_OnTick(object state)
        {
            if (Deleted)
                return;

            var states = (object[])state;
            Mobile from = (Mobile)states[0];
            int timer = (int)states[1];

            object parent = FindParent(from);

            if (timer == 0)
            {
                Point3D loc;
                Map map;

                if (parent is Item)
                {
                    Item item = (Item)parent;
                    loc = item.GetWorldLocation();
                    map = item.Map;
                }
                else if (parent is Mobile)
                {
                    Mobile m = (Mobile)parent;
                    loc = m.Location;
                    map = m.Map;
                }
                else
                {
                    return;
                }

                CustomExplode(from, true, loc, map);
            }
            else
            {
                if (parent is Item)
                {
                    ((Item)parent).PublicOverheadMessage(MessageType.Regular, 0x22, false, timer.ToString());
                }
                else if (parent is Mobile)
                {
                    ((Mobile)parent).PublicOverheadMessage(MessageType.Regular, 0x22, false, timer.ToString());
                }

                states[1] = timer - 1;
            }
        }

        private void CustomExplode(Mobile from, bool direct, Point3D loc, Map map)
        {
            if (Deleted || map == null)
                return;

            // Call base explosion for damage
            base.Explode(from, direct, loc, map);

            // Add unique mechanical effects
            Effects.PlaySound(loc, map, 0x307);
            Effects.SendLocationEffect(loc, map, 0x36B0, 9, 10, 2401, 0);

            // Create mechanical sparks effect
            for (int i = 0; i < 3; i++)
            {
                Timer.DelayCall(TimeSpan.FromMilliseconds(i * 200), () =>
                {
                    if (map != null)
                    {
                        Effects.SendLocationParticles(
                            EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                            0x3728, 10, 10, 2401, 0, 5052, 0);
                    }
                });
            }

            // Create smoke cloud that lingers for 5 seconds
            CreateSmokeCloud(loc, map, from, 5);
        }

        private void CreateSmokeCloud(Point3D loc, Map map, Mobile from, int duration)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(1.0), duration, () =>
            {
                if (map == null)
                    return;

                Effects.SendLocationParticles(
                    EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                    0x3728, 15, 20, 1109, 0, 5029, 0);

                IPooledEnumerable eable = map.GetMobilesInRange(loc, 2);
                List<Mobile> targets = new List<Mobile>();

                foreach (Mobile m in eable)
                {
                    if (m != null && m.Alive && m != from && from != null && from.CanBeHarmful(m, false))
                        targets.Add(m);
                }
                eable.Free();

                foreach (Mobile target in targets)
                {
                    from.DoHarmful(target);
                    int smokeDamage = Utility.RandomMinMax(2, 5);
                    AOS.Damage(target, from, smokeDamage, 0, 0, 0, 0, 0, Server.DamageType.SpellAOE);
                    target.FixedParticles(0x374A, 5, 10, 5021, 1109, 0, EffectLayer.Waist);
                }
            });
        }

        private class VystiaThrowTarget : Target
        {
            private readonly VystiaLesserEngineeringExplosive m_Potion;

            public VystiaThrowTarget(VystiaLesserEngineeringExplosive potion)
                : base(12, true, TargetFlags.None)
            {
                m_Potion = potion;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Potion.Deleted || m_Potion.Map == Map.Internal)
                    return;

                IPoint3D p = targeted as IPoint3D;
                if (p == null)
                    return;

                Map map = from.Map;
                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref p);
                from.RevealingAction();

                IEntity to = new Entity(Serial.Zero, new Point3D(p), map);
                if (p is Mobile)
                    to = (Mobile)p;

                Effects.SendMovingEffect(from, to, m_Potion.ItemID, 7, 0, false, false, m_Potion.Hue, 0);

                if (m_Potion.Amount > 1)
                    Mobile.LiftItemDupe(m_Potion, 1);

                m_Potion.Internalize();
                Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerStateCallback(m_Potion.VystiaReposition_OnTick), new object[] { from, p, map });
            }
        }

        private void VystiaReposition_OnTick(object state)
        {
            if (Deleted)
                return;

            var states = (object[])state;
            Mobile from = (Mobile)states[0];
            IPoint3D p = (IPoint3D)states[1];
            Map map = (Map)states[2];

            Point3D loc = new Point3D(p);
            MoveToWorld(loc, map);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaEngineeringExplosive : BaseExplosionPotion
    {
        [Constructable]
        public VystiaEngineeringExplosive()
            : base(PotionEffect.Explosion)
        {
            Hue = 2401;
            Name = "Mechanical Explosive";
        }

        public VystiaEngineeringExplosive(Serial serial)
            : base(serial)
        { }

        public override int MinDamage { get { return 10; } }
        public override int MaxDamage { get { return 20; } }

        public override void Drink(Mobile from)
        {
            if (Core.AOS && (from.Paralyzed || from.Frozen || (from.Spell != null && from.Spell.IsCasting)))
            {
                from.SendLocalizedMessage(1062725);
                return;
            }

            Stackable = false;
            from.RevealingAction();
            from.Target = new VystiaThrowTarget(this);

            from.SendLocalizedMessage(500236);

            Timer.DelayCall(
                TimeSpan.FromSeconds(1.0),
                TimeSpan.FromSeconds(1.25),
                5,
                new TimerStateCallback(VystiaDetonate_OnTick),
                new object[] { from, 3 });
        }

        private void VystiaDetonate_OnTick(object state)
        {
            if (Deleted)
                return;

            var states = (object[])state;
            Mobile from = (Mobile)states[0];
            int timer = (int)states[1];

            object parent = FindParent(from);

            if (timer == 0)
            {
                Point3D loc;
                Map map;

                if (parent is Item)
                {
                    Item item = (Item)parent;
                    loc = item.GetWorldLocation();
                    map = item.Map;
                }
                else if (parent is Mobile)
                {
                    Mobile m = (Mobile)parent;
                    loc = m.Location;
                    map = m.Map;
                }
                else
                {
                    return;
                }

                CustomExplode(from, true, loc, map);
            }
            else
            {
                if (parent is Item)
                {
                    ((Item)parent).PublicOverheadMessage(MessageType.Regular, 0x22, false, timer.ToString());
                }
                else if (parent is Mobile)
                {
                    ((Mobile)parent).PublicOverheadMessage(MessageType.Regular, 0x22, false, timer.ToString());
                }

                states[1] = timer - 1;
            }
        }

        private void CustomExplode(Mobile from, bool direct, Point3D loc, Map map)
        {
            if (Deleted || map == null)
                return;

            base.Explode(from, direct, loc, map);
            Effects.PlaySound(loc, map, 0x307);
            Effects.SendLocationEffect(loc, map, 0x36B0, 9, 10, 2401, 0);

            for (int i = 0; i < 3; i++)
            {
                Timer.DelayCall(TimeSpan.FromMilliseconds(i * 200), () =>
                {
                    if (map != null)
                    {
                        Effects.SendLocationParticles(
                            EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                            0x3728, 10, 10, 2401, 0, 5052, 0);
                    }
                });
            }

            CreateSmokeCloud(loc, map, from, 5);
        }

        private void CreateSmokeCloud(Point3D loc, Map map, Mobile from, int duration)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(1.0), duration, () =>
            {
                if (map == null)
                    return;

                Effects.SendLocationParticles(
                    EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                    0x3728, 15, 20, 1109, 0, 5029, 0);

                IPooledEnumerable eable = map.GetMobilesInRange(loc, 2);
                List<Mobile> targets = new List<Mobile>();

                foreach (Mobile m in eable)
                {
                    if (m != null && m.Alive && m != from && from != null && from.CanBeHarmful(m, false))
                        targets.Add(m);
                }
                eable.Free();

                foreach (Mobile target in targets)
                {
                    from.DoHarmful(target);
                    int smokeDamage = Utility.RandomMinMax(2, 5);
                    AOS.Damage(target, from, smokeDamage, 0, 0, 0, 0, 0, Server.DamageType.SpellAOE);
                    target.FixedParticles(0x374A, 5, 10, 5021, 1109, 0, EffectLayer.Waist);
                }
            });
        }

        private class VystiaThrowTarget : Target
        {
            private readonly VystiaEngineeringExplosive m_Potion;

            public VystiaThrowTarget(VystiaEngineeringExplosive potion)
                : base(12, true, TargetFlags.None)
            {
                m_Potion = potion;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Potion.Deleted || m_Potion.Map == Map.Internal)
                    return;

                IPoint3D p = targeted as IPoint3D;
                if (p == null)
                    return;

                Map map = from.Map;
                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref p);
                from.RevealingAction();

                IEntity to = new Entity(Serial.Zero, new Point3D(p), map);
                if (p is Mobile)
                    to = (Mobile)p;

                Effects.SendMovingEffect(from, to, m_Potion.ItemID, 7, 0, false, false, m_Potion.Hue, 0);

                if (m_Potion.Amount > 1)
                    Mobile.LiftItemDupe(m_Potion, 1);

                m_Potion.Internalize();
                Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerStateCallback(m_Potion.VystiaReposition_OnTick), new object[] { from, p, map });
            }
        }

        private void VystiaReposition_OnTick(object state)
        {
            if (Deleted)
                return;

            var states = (object[])state;
            Mobile from = (Mobile)states[0];
            IPoint3D p = (IPoint3D)states[1];
            Map map = (Map)states[2];

            Point3D loc = new Point3D(p);
            MoveToWorld(loc, map);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaGreaterEngineeringExplosive : BaseExplosionPotion
    {
        [Constructable]
        public VystiaGreaterEngineeringExplosive()
            : base(PotionEffect.ExplosionGreater)
        {
            Hue = 2401;
            Name = "Large Mechanical Explosive";
        }

        public VystiaGreaterEngineeringExplosive(Serial serial)
            : base(serial)
        { }

        public override int MinDamage { get { return Core.AOS ? 20 : 15; } }
        public override int MaxDamage { get { return Core.AOS ? 40 : 30; } }

        public override void Drink(Mobile from)
        {
            if (Core.AOS && (from.Paralyzed || from.Frozen || (from.Spell != null && from.Spell.IsCasting)))
            {
                from.SendLocalizedMessage(1062725);
                return;
            }

            Stackable = false;
            from.RevealingAction();
            from.Target = new VystiaThrowTarget(this);

            from.SendLocalizedMessage(500236);

            Timer.DelayCall(
                TimeSpan.FromSeconds(1.0),
                TimeSpan.FromSeconds(1.25),
                5,
                new TimerStateCallback(VystiaDetonate_OnTick),
                new object[] { from, 3 });
        }

        private void VystiaDetonate_OnTick(object state)
        {
            if (Deleted)
                return;

            var states = (object[])state;
            Mobile from = (Mobile)states[0];
            int timer = (int)states[1];

            object parent = FindParent(from);

            if (timer == 0)
            {
                Point3D loc;
                Map map;

                if (parent is Item)
                {
                    Item item = (Item)parent;
                    loc = item.GetWorldLocation();
                    map = item.Map;
                }
                else if (parent is Mobile)
                {
                    Mobile m = (Mobile)parent;
                    loc = m.Location;
                    map = m.Map;
                }
                else
                {
                    return;
                }

                CustomExplode(from, true, loc, map);
            }
            else
            {
                if (parent is Item)
                {
                    ((Item)parent).PublicOverheadMessage(MessageType.Regular, 0x22, false, timer.ToString());
                }
                else if (parent is Mobile)
                {
                    ((Mobile)parent).PublicOverheadMessage(MessageType.Regular, 0x22, false, timer.ToString());
                }

                states[1] = timer - 1;
            }
        }

        private void CustomExplode(Mobile from, bool direct, Point3D loc, Map map)
        {
            if (Deleted || map == null)
                return;

            base.Explode(from, direct, loc, map);
            Effects.PlaySound(loc, map, 0x307);
            Effects.SendLocationEffect(loc, map, 0x36B0, 12, 15, 2401, 0);

            for (int i = 0; i < 5; i++)
            {
                Timer.DelayCall(TimeSpan.FromMilliseconds(i * 150), () =>
                {
                    if (map != null)
                    {
                        Effects.SendLocationParticles(
                            EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                            0x3728, 15, 15, 2401, 0, 5052, 0);
                    }
                });
            }

            CreateSmokeCloud(loc, map, from, 8);
        }

        private void CreateSmokeCloud(Point3D loc, Map map, Mobile from, int duration)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(1.0), duration, () =>
            {
                if (map == null)
                    return;

                Effects.SendLocationParticles(
                    EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                    0x3728, 20, 25, 1109, 0, 5029, 0);

                IPooledEnumerable eable = map.GetMobilesInRange(loc, 3);
                List<Mobile> targets = new List<Mobile>();

                foreach (Mobile m in eable)
                {
                    if (m != null && m.Alive && m != from && from != null && from.CanBeHarmful(m, false))
                        targets.Add(m);
                }
                eable.Free();

                foreach (Mobile target in targets)
                {
                    from.DoHarmful(target);
                    int smokeDamage = Utility.RandomMinMax(3, 7);
                    AOS.Damage(target, from, smokeDamage, 0, 0, 0, 0, 0, Server.DamageType.SpellAOE);
                    target.FixedParticles(0x374A, 5, 10, 5021, 1109, 0, EffectLayer.Waist);
                }
            });
        }

        private class VystiaThrowTarget : Target
        {
            private readonly VystiaGreaterEngineeringExplosive m_Potion;

            public VystiaThrowTarget(VystiaGreaterEngineeringExplosive potion)
                : base(12, true, TargetFlags.None)
            {
                m_Potion = potion;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Potion.Deleted || m_Potion.Map == Map.Internal)
                    return;

                IPoint3D p = targeted as IPoint3D;
                if (p == null)
                    return;

                Map map = from.Map;
                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref p);
                from.RevealingAction();

                IEntity to = new Entity(Serial.Zero, new Point3D(p), map);
                if (p is Mobile)
                    to = (Mobile)p;

                Effects.SendMovingEffect(from, to, m_Potion.ItemID, 7, 0, false, false, m_Potion.Hue, 0);

                if (m_Potion.Amount > 1)
                    Mobile.LiftItemDupe(m_Potion, 1);

                m_Potion.Internalize();
                Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerStateCallback(m_Potion.VystiaReposition_OnTick), new object[] { from, p, map });
            }
        }

        private void VystiaReposition_OnTick(object state)
        {
            if (Deleted)
                return;

            var states = (object[])state;
            Mobile from = (Mobile)states[0];
            IPoint3D p = (IPoint3D)states[1];
            Map map = (Map)states[2];

            Point3D loc = new Point3D(p);
            MoveToWorld(loc, map);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Vystia Transmutation Explosives - Creates toxic cloud with poison effect
    /// Used by Transmutation crafting system
    /// </summary>
    public class VystiaLesserTransmutationExplosive : BaseExplosionPotion
    {
        [Constructable]
        public VystiaLesserTransmutationExplosive()
            : base(PotionEffect.ExplosionLesser)
        {
            Hue = 0x81D; // Toxic green/purple hue
            Name = "Lesser Toxic Explosive Flask";
        }

        public VystiaLesserTransmutationExplosive(Serial serial)
            : base(serial)
        { }

        public override int MinDamage { get { return 5; } }
        public override int MaxDamage { get { return 10; } }

        public override void Drink(Mobile from)
        {
            if (Core.AOS && (from.Paralyzed || from.Frozen || (from.Spell != null && from.Spell.IsCasting)))
            {
                from.SendLocalizedMessage(1062725);
                return;
            }

            Stackable = false;
            from.RevealingAction();
            from.Target = new VystiaThrowTarget(this);

            from.SendLocalizedMessage(500236);

            Timer.DelayCall(
                TimeSpan.FromSeconds(1.0),
                TimeSpan.FromSeconds(1.25),
                5,
                new TimerStateCallback(VystiaDetonate_OnTick),
                new object[] { from, 3 });
        }

        private void VystiaDetonate_OnTick(object state)
        {
            if (Deleted)
                return;

            var states = (object[])state;
            Mobile from = (Mobile)states[0];
            int timer = (int)states[1];

            object parent = FindParent(from);

            if (timer == 0)
            {
                Point3D loc;
                Map map;

                if (parent is Item)
                {
                    Item item = (Item)parent;
                    loc = item.GetWorldLocation();
                    map = item.Map;
                }
                else if (parent is Mobile)
                {
                    Mobile m = (Mobile)parent;
                    loc = m.Location;
                    map = m.Map;
                }
                else
                {
                    return;
                }

                CustomExplode(from, true, loc, map);
            }
            else
            {
                if (parent is Item)
                {
                    ((Item)parent).PublicOverheadMessage(MessageType.Regular, 0x22, false, timer.ToString());
                }
                else if (parent is Mobile)
                {
                    ((Mobile)parent).PublicOverheadMessage(MessageType.Regular, 0x22, false, timer.ToString());
                }

                states[1] = timer - 1;
            }
        }

        private void CustomExplode(Mobile from, bool direct, Point3D loc, Map map)
        {
            if (Deleted || map == null)
                return;

            base.Explode(from, direct, loc, map);
            Effects.PlaySound(loc, map, 0x230);
            Effects.SendLocationEffect(loc, map, 0x36B0, 9, 10, 0x81D, 0);
            CreateToxicCloud(loc, map, from, 8);
        }

        private void CreateToxicCloud(Point3D loc, Map map, Mobile from, int duration)
        {
            int tickCount = 0; // Track current tick number
            Timer.DelayCall(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(1.0), duration, () =>
            {
                if (map == null)
                    return;

                tickCount++; // Increment tick counter

                Effects.SendLocationParticles(
                    EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                    0x3728, 15, 20, 0x1A, 0, 5029, 0);

                IPooledEnumerable eable = map.GetMobilesInRange(loc, 2);
                List<Mobile> targets = new List<Mobile>();

                foreach (Mobile m in eable)
                {
                    if (m != null && m.Alive && m != from && from != null && from.CanBeHarmful(m, false))
                        targets.Add(m);
                }
                eable.Free();

                foreach (Mobile target in targets)
                {
                    from.DoHarmful(target);
                    int toxicDamage = Utility.RandomMinMax(3, 7);
                    AOS.Damage(target, from, toxicDamage, 0, 0, 0, 100, 0, Server.DamageType.SpellAOE);
                    
                    // Apply poison on first tick and then every 2 ticks (1st, 3rd, 5th, etc.)
                    if (tickCount == 1 || tickCount % 2 == 1)
                    {
                        target.ApplyPoison(from, Poison.Regular);
                    }

                    target.FixedParticles(0x374A, 5, 10, 5021, 0x1A, 0, EffectLayer.Waist);
                    target.SendMessage(0x22, $"You are poisoned by the toxic cloud! ({toxicDamage} damage)");
                }
            });
        }

        private class VystiaThrowTarget : Target
        {
            private readonly VystiaLesserTransmutationExplosive m_Potion;

            public VystiaThrowTarget(VystiaLesserTransmutationExplosive potion)
                : base(12, true, TargetFlags.None)
            {
                m_Potion = potion;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Potion.Deleted || m_Potion.Map == Map.Internal)
                    return;

                IPoint3D p = targeted as IPoint3D;
                if (p == null)
                    return;

                Map map = from.Map;
                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref p);
                from.RevealingAction();

                IEntity to = new Entity(Serial.Zero, new Point3D(p), map);
                if (p is Mobile)
                    to = (Mobile)p;

                Effects.SendMovingEffect(from, to, m_Potion.ItemID, 7, 0, false, false, m_Potion.Hue, 0);

                if (m_Potion.Amount > 1)
                    Mobile.LiftItemDupe(m_Potion, 1);

                m_Potion.Internalize();
                Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerStateCallback(m_Potion.VystiaReposition_OnTick), new object[] { from, p, map });
            }
        }

        private void VystiaReposition_OnTick(object state)
        {
            if (Deleted)
                return;

            var states = (object[])state;
            Mobile from = (Mobile)states[0];
            IPoint3D p = (IPoint3D)states[1];
            Map map = (Map)states[2];

            Point3D loc = new Point3D(p);
            MoveToWorld(loc, map);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaTransmutationExplosive : BaseExplosionPotion
    {
        [Constructable]
        public VystiaTransmutationExplosive()
            : base(PotionEffect.Explosion)
        {
            Hue = 0x81D;
            Name = "Toxic Explosive Flask";
        }

        public VystiaTransmutationExplosive(Serial serial)
            : base(serial)
        { }

        public override int MinDamage { get { return 10; } }
        public override int MaxDamage { get { return 20; } }

        public override void Drink(Mobile from)
        {
            if (Core.AOS && (from.Paralyzed || from.Frozen || (from.Spell != null && from.Spell.IsCasting)))
            {
                from.SendLocalizedMessage(1062725);
                return;
            }

            Stackable = false;
            from.RevealingAction();
            from.Target = new VystiaThrowTarget(this);

            from.SendLocalizedMessage(500236);

            Timer.DelayCall(
                TimeSpan.FromSeconds(1.0),
                TimeSpan.FromSeconds(1.25),
                5,
                new TimerStateCallback(VystiaDetonate_OnTick),
                new object[] { from, 3 });
        }

        private void VystiaDetonate_OnTick(object state)
        {
            if (Deleted)
                return;

            var states = (object[])state;
            Mobile from = (Mobile)states[0];
            int timer = (int)states[1];

            object parent = FindParent(from);

            if (timer == 0)
            {
                Point3D loc;
                Map map;

                if (parent is Item)
                {
                    Item item = (Item)parent;
                    loc = item.GetWorldLocation();
                    map = item.Map;
                }
                else if (parent is Mobile)
                {
                    Mobile m = (Mobile)parent;
                    loc = m.Location;
                    map = m.Map;
                }
                else
                {
                    return;
                }

                CustomExplode(from, true, loc, map);
            }
            else
            {
                if (parent is Item)
                {
                    ((Item)parent).PublicOverheadMessage(MessageType.Regular, 0x22, false, timer.ToString());
                }
                else if (parent is Mobile)
                {
                    ((Mobile)parent).PublicOverheadMessage(MessageType.Regular, 0x22, false, timer.ToString());
                }

                states[1] = timer - 1;
            }
        }

        private void CustomExplode(Mobile from, bool direct, Point3D loc, Map map)
        {
            if (Deleted || map == null)
                return;

            base.Explode(from, direct, loc, map);
            Effects.PlaySound(loc, map, 0x230);
            Effects.SendLocationEffect(loc, map, 0x36B0, 9, 10, 0x81D, 0);
            CreateToxicCloud(loc, map, from, 8);
        }

        private void CreateToxicCloud(Point3D loc, Map map, Mobile from, int duration)
        {
            int tickCount = 0; // Track current tick number
            Timer.DelayCall(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(1.0), duration, () =>
            {
                if (map == null)
                    return;

                tickCount++; // Increment tick counter

                Effects.SendLocationParticles(
                    EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                    0x3728, 15, 20, 0x1A, 0, 5029, 0);

                IPooledEnumerable eable = map.GetMobilesInRange(loc, 2);
                List<Mobile> targets = new List<Mobile>();

                foreach (Mobile m in eable)
                {
                    if (m != null && m.Alive && m != from && from != null && from.CanBeHarmful(m, false))
                        targets.Add(m);
                }
                eable.Free();

                foreach (Mobile target in targets)
                {
                    from.DoHarmful(target);
                    int toxicDamage = Utility.RandomMinMax(3, 7);
                    AOS.Damage(target, from, toxicDamage, 0, 0, 0, 100, 0, Server.DamageType.SpellAOE);
                    
                    // Apply poison on first tick and then every 2 ticks (1st, 3rd, 5th, etc.)
                    if (tickCount == 1 || tickCount % 2 == 1)
                    {
                        target.ApplyPoison(from, Poison.Regular);
                    }

                    target.FixedParticles(0x374A, 5, 10, 5021, 0x1A, 0, EffectLayer.Waist);
                    target.SendMessage(0x22, $"You are poisoned by the toxic cloud! ({toxicDamage} damage)");
                }
            });
        }

        private class VystiaThrowTarget : Target
        {
            private readonly VystiaTransmutationExplosive m_Potion;

            public VystiaThrowTarget(VystiaTransmutationExplosive potion)
                : base(12, true, TargetFlags.None)
            {
                m_Potion = potion;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Potion.Deleted || m_Potion.Map == Map.Internal)
                    return;

                IPoint3D p = targeted as IPoint3D;
                if (p == null)
                    return;

                Map map = from.Map;
                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref p);
                from.RevealingAction();

                IEntity to = new Entity(Serial.Zero, new Point3D(p), map);
                if (p is Mobile)
                    to = (Mobile)p;

                Effects.SendMovingEffect(from, to, m_Potion.ItemID, 7, 0, false, false, m_Potion.Hue, 0);

                if (m_Potion.Amount > 1)
                    Mobile.LiftItemDupe(m_Potion, 1);

                m_Potion.Internalize();
                Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerStateCallback(m_Potion.VystiaReposition_OnTick), new object[] { from, p, map });
            }
        }

        private void VystiaReposition_OnTick(object state)
        {
            if (Deleted)
                return;

            var states = (object[])state;
            Mobile from = (Mobile)states[0];
            IPoint3D p = (IPoint3D)states[1];
            Map map = (Map)states[2];

            Point3D loc = new Point3D(p);
            MoveToWorld(loc, map);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VystiaGreaterTransmutationExplosive : BaseExplosionPotion
    {
        [Constructable]
        public VystiaGreaterTransmutationExplosive()
            : base(PotionEffect.ExplosionGreater)
        {
            Hue = 0x81D;
            Name = "Greater Toxic Explosive Flask";
        }

        public VystiaGreaterTransmutationExplosive(Serial serial)
            : base(serial)
        { }

        public override int MinDamage { get { return Core.AOS ? 20 : 15; } }
        public override int MaxDamage { get { return Core.AOS ? 40 : 30; } }

        public override void Drink(Mobile from)
        {
            if (Core.AOS && (from.Paralyzed || from.Frozen || (from.Spell != null && from.Spell.IsCasting)))
            {
                from.SendLocalizedMessage(1062725);
                return;
            }

            Stackable = false;
            from.RevealingAction();
            from.Target = new VystiaThrowTarget(this);

            from.SendLocalizedMessage(500236);

            Timer.DelayCall(
                TimeSpan.FromSeconds(1.0),
                TimeSpan.FromSeconds(1.25),
                5,
                new TimerStateCallback(VystiaDetonate_OnTick),
                new object[] { from, 3 });
        }

        private void VystiaDetonate_OnTick(object state)
        {
            if (Deleted)
                return;

            var states = (object[])state;
            Mobile from = (Mobile)states[0];
            int timer = (int)states[1];

            object parent = FindParent(from);

            if (timer == 0)
            {
                Point3D loc;
                Map map;

                if (parent is Item)
                {
                    Item item = (Item)parent;
                    loc = item.GetWorldLocation();
                    map = item.Map;
                }
                else if (parent is Mobile)
                {
                    Mobile m = (Mobile)parent;
                    loc = m.Location;
                    map = m.Map;
                }
                else
                {
                    return;
                }

                CustomExplode(from, true, loc, map);
            }
            else
            {
                if (parent is Item)
                {
                    ((Item)parent).PublicOverheadMessage(MessageType.Regular, 0x22, false, timer.ToString());
                }
                else if (parent is Mobile)
                {
                    ((Mobile)parent).PublicOverheadMessage(MessageType.Regular, 0x22, false, timer.ToString());
                }

                states[1] = timer - 1;
            }
        }

        private void CustomExplode(Mobile from, bool direct, Point3D loc, Map map)
        {
            if (Deleted || map == null)
                return;

            base.Explode(from, direct, loc, map);
            Effects.PlaySound(loc, map, 0x230);
            Effects.SendLocationEffect(loc, map, 0x36B0, 12, 15, 0x81D, 0);
            CreateToxicCloud(loc, map, from, 12);
        }

        private void CreateToxicCloud(Point3D loc, Map map, Mobile from, int duration)
        {
            int tickCount = 0; // Track current tick number
            Timer.DelayCall(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(1.0), duration, () =>
            {
                if (map == null)
                    return;

                tickCount++; // Increment tick counter

                Effects.SendLocationParticles(
                    EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                    0x3728, 20, 25, 0x1A, 0, 5029, 0);

                IPooledEnumerable eable = map.GetMobilesInRange(loc, 3);
                List<Mobile> targets = new List<Mobile>();

                foreach (Mobile m in eable)
                {
                    if (m != null && m.Alive && m != from && from != null && from.CanBeHarmful(m, false))
                        targets.Add(m);
                }
                eable.Free();

                foreach (Mobile target in targets)
                {
                    from.DoHarmful(target);
                    int toxicDamage = Utility.RandomMinMax(5, 10);
                    AOS.Damage(target, from, toxicDamage, 0, 0, 0, 100, 0, Server.DamageType.SpellAOE);
                    
                    // Apply poison on first tick and then every 2 ticks (1st, 3rd, 5th, etc.)
                    if (tickCount == 1 || tickCount % 2 == 1)
                    {
                        target.ApplyPoison(from, Poison.Greater);
                    }

                    target.FixedParticles(0x374A, 5, 10, 5021, 0x1A, 0, EffectLayer.Waist);
                    target.SendMessage(0x22, $"You are heavily poisoned by the toxic cloud! ({toxicDamage} damage)");
                }
            });
        }

        private class VystiaThrowTarget : Target
        {
            private readonly VystiaGreaterTransmutationExplosive m_Potion;

            public VystiaThrowTarget(VystiaGreaterTransmutationExplosive potion)
                : base(12, true, TargetFlags.None)
            {
                m_Potion = potion;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Potion.Deleted || m_Potion.Map == Map.Internal)
                    return;

                IPoint3D p = targeted as IPoint3D;
                if (p == null)
                    return;

                Map map = from.Map;
                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref p);
                from.RevealingAction();

                IEntity to = new Entity(Serial.Zero, new Point3D(p), map);
                if (p is Mobile)
                    to = (Mobile)p;

                Effects.SendMovingEffect(from, to, m_Potion.ItemID, 7, 0, false, false, m_Potion.Hue, 0);

                if (m_Potion.Amount > 1)
                    Mobile.LiftItemDupe(m_Potion, 1);

                m_Potion.Internalize();
                Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerStateCallback(m_Potion.VystiaReposition_OnTick), new object[] { from, p, map });
            }
        }

        private void VystiaReposition_OnTick(object state)
        {
            if (Deleted)
                return;

            var states = (object[])state;
            Mobile from = (Mobile)states[0];
            IPoint3D p = (IPoint3D)states[1];
            Map map = (Map)states[2];

            Point3D loc = new Point3D(p);
            MoveToWorld(loc, map);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
