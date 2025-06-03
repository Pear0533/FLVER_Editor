namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// A Component which contains stats for weapons.
        /// </summary>
        public class WeaponComponent
        {
            /// <summary>
            /// The firing mode of the weapon determining how it behaves when it is fired.
            /// </summary>
            // Originally called "prog" for "program"
            public enum FiringMode : byte
            {
                /// <summary>
                /// The weapon can be fired by holding the trigger without needing to let go.
                /// </summary>
                // Originally called "GUN" for "gun"
                GUN = 0,

                /// <summary>
                /// The weapon requires the trigger to be pressed each time it is fired.
                /// </summary>
                // Originally called "MIS" for "missile"
                MIS = 1,

                /// <summary>
                /// The weapon can be fired by holding the trigger without needing to let go.<br/>
                /// The weapon goes into the ready position after pressing the firing button while standing still on ground.<br/>
                /// The ready position lets the weapon fire faster and allows for wider attack angles.
                /// </summary>
                // Originally called "CAN" for "cannon"
                CAN = 2,

                /// <summary>
                /// The weapon is melee and lunges towards targets.
                /// </summary>
                // Originally called "BLD" for "blade"
                BLD = 4,

                /// <summary>
                /// The weapon is melee and does not lunge.
                /// </summary>
                // Originally called "PILE" for "pilebunker"
                PILE = 5
            }

            /// <summary>
            /// Unknown; Is set on the described weapons in ACFA; Everything else appears to be set to Weapon or 0.
            /// </summary>
            public enum WeaponType : ushort
            {
                /// <summary>
                /// The default value is set.
                /// </summary>
                Weapon = 0,

                /// <summary>
                /// This weapon is Dozar.
                /// </summary>
                Dozar = 1,

                /// <summary>
                /// This weapon is a parry blade/physical blade/piledriver.
                /// </summary>
                Pile = 4,

                /// <summary>
                /// This weapon is a ballistic weapon.
                /// </summary>
                Ballistic = 5,

                /// <summary>
                /// This weapon is an energy weapon.
                /// </summary>
                Energy = 6,

                /// <summary>
                /// This weapon is a missile weapon.
                /// </summary>
                // Not set on all missiles?
                Missile = 7,

                /// <summary>
                /// This weapon is a kojima weapon.
                /// </summary>
                Kojima = 9
            }

            /// <summary>
            /// The different damage types.
            /// </summary>
            public enum DamageType : byte
            {
                /// <summary>
                /// A rigid type of damage.
                /// </summary>
                Rigid = 0,

                /// <summary>
                /// An energy type of damage.
                /// </summary>
                Energy = 1
            }

            /// <summary>
            /// The firing mode of the weapon determining how it behaves when it is fired.
            /// </summary>
            public FiringMode WeaponFiringMode { get; set; }

            /// <summary>
            /// Whether or not the weapon can lock on.
            /// </summary>
            public bool CanLockOn { get; set; }

            /// <summary>
            /// The time it takes for missiles to lock on.
            /// </summary>
            public byte MissileLockTime { get; set; }

            /// <summary>
            /// Unknown; Is always 0 or 1.
            /// </summary>
            public bool Unk03 { get; set; }

            /// <summary>
            /// The effective firing range of the weapon.
            /// Larger values allow you to target more distant enemies.
            /// </summary>
            public ushort FiringRange { get; set; }

            /// <summary>
            /// Ability to wield close quarter combat weapons. 
            /// Larger values indicate increased weapon maneuverability,
            /// improving close combat effectiveness.
            /// </summary>
            public ushort MeleeAbility { get; set; }

            /// <summary>
            /// The bullet ID from bullet params to generate when firing this weapon.
            /// </summary>
            public uint BulletID { get; set; }

            /// <summary>
            /// What SFX to use, determines the appearance of the bullets fired from this weapon.
            /// </summary>
            public uint SFXID { get; set; }

            /// <summary>
            /// What landing effect to use.
            /// </summary>
            public uint HitEffectID { get; set; }

            /// <summary>
            /// Velocity of fired ammunition.
            /// </summary>
            public float BallisticsVelocity { get; set; }

            /// <summary>
            /// Energy cost upon activation.
            /// </summary>
            public float ENCost { get; set; }

            /// <summary>
            /// Unknown; Multi processing? Assumed from value and positioning.
            /// </summary>
            public bool MultiProc { get; set; }

            /// <summary>
            /// Number of projectiles fired in a single launch or shot.
            /// </summary>
            public byte ProjectileCount { get; set; }

            /// <summary>
            /// Number of projectiles fired in a row in a single launch.
            /// </summary>
            public byte ContinuousFireCount { get; set; }

            /// <summary>
            /// Unknown; Is always 0.
            /// </summary>
            public byte Unk1F { get; set; }

            /// <summary>
            /// Unknown; Is always 0; May be a part of AutoInterval as a short.
            /// </summary>
            public ushort Unk20 { get; set; }

            /// <summary>
            /// How long to wait between continuous shots.
            /// </summary>
            public ushort AutoInterval { get; set; }

            /// <summary>
            /// After firing, time required before weapon can be fired again.
            /// </summary>
            public ushort FireRate { get; set; }

            /// <summary>
            /// Amount of recoil when firing the weapon.
            /// </summary>
            public ushort Recoil { get; set; }

            /// <summary>
            /// The cost of firing a single projectile (COAM Units).
            /// </summary>
            public ushort CostPerRound { get; set; }

            /// <summary>
            /// Indicates the amount that fired shots will deviate from the targeted point.
            /// The larger the value, the more accurate the weapon.
            /// Currently unknown how this value is factored.
            /// </summary>
            public ushort ShotPrecision { get; set; }

            /// <summary>
            /// Total number of magazines available.
            /// </summary>
            public ushort NumberofMagazines { get; set; }

            /// <summary>
            /// The number of rounds in a single magazine.
            /// </summary>
            public ushort MagazineCapacity { get; set; }

            /// <summary>
            /// How fast this weapon reloads.
            /// </summary>
            public ushort MagazineReloadTime { get; set; }

            /// <summary>
            /// Unknown; Is set on the described weapons in ACFA; Everything else appears to be set to <see cref="WeaponType.Weapon"/>.
            /// </summary>
            public WeaponType Weapon_Type { get; set; }

            /// <summary>
            /// Amount of time required to recharge Kojima particles. 
            /// Attack power increases related to charge time.
            /// </summary>
            public ushort ChargeTime { get; set; }

            /// <summary>
            /// While Kojima Particles are being charged, the amount of KP consumed over time.
            /// </summary>
            public ushort KPChargeCost { get; set; }

            /// <summary>
            /// Unknown; Related to the maximum damage rate of kojima somehow.
            /// </summary>
            public float KojimaMaxDamageRate { get; set; }

            /// <summary>
            /// After use, time required before the weapon is available.
            /// </summary>
            public ushort AttackLatency { get; set; }

            /// <summary>
            /// Unknown; Is always 0.
            /// </summary>
            public ushort Unk3E { get; set; }

            /// <summary>
            /// The damage type of the weapon.
            /// </summary>
            public DamageType Damage_Type { get; set; }

            /// <summary>
            /// Unknown; From number and positioning from AC4 txt files, assumed to be DamagePierce. Used on Pile and Blade weapons.
            /// </summary>
            public bool DamagePierce { get; set; }

            /// <summary>
            /// Unknown; Is always 0 or 1.
            /// </summary>
            public bool Unk41 { get; set; }

            /// <summary>
            /// Unknown; From previous number and positioning from AC4 txt files, assumed to be DamageRadial; Is always 0.
            /// </summary>
            public bool DamageRadial { get; set; }

            /// <summary>
            /// Indicates attack power of this type of weaponry.
            /// The larger this value, the greater damage the AC can deliver.
            /// </summary>
            public float AttackPower { get; set; }

            /// <summary>
            /// Amount of force delivered upon striking the target.
            /// The larger the value, the greater the impact.
            /// </summary>
            public float ImpactForce { get; set; }

            /// <summary>
            /// Ability to reduce PA when striking the target.
            /// The larger the value, the more damage PA can receive.
            /// </summary>
            public float PAAttentuation { get; set; }

            /// <summary>
            /// Ability to successfully penetrate PA.
            /// The larger the value, the less protection PA affords.
            /// </summary>
            public float PAPenetration { get; set; }

            /// <summary>
            /// Makes a new <see cref="WeaponComponent"/>.
            /// </summary>
            public WeaponComponent()
            {

            }

            /// <summary>
            /// Reads a Projectile component from a stream.
            /// </summary>
            /// <param name="br">A binary reader.</param>
            internal WeaponComponent(BinaryReaderEx br)
            {
                WeaponFiringMode = br.ReadEnum8<FiringMode>();
                CanLockOn = br.ReadBoolean();
                MissileLockTime = br.ReadByte();
                Unk03 = br.ReadBoolean();
                FiringRange = br.ReadUInt16();
                MeleeAbility = br.ReadUInt16();
                BulletID = br.ReadUInt32();
                SFXID = br.ReadUInt32();
                HitEffectID = br.ReadUInt32();
                BallisticsVelocity = br.ReadSingle();
                ENCost = br.ReadSingle();
                MultiProc = br.ReadBoolean();
                ProjectileCount = br.ReadByte();
                ContinuousFireCount = br.ReadByte();
                Unk1F = br.ReadByte();
                Unk20 = br.ReadUInt16();
                AutoInterval = br.ReadUInt16();
                FireRate = br.ReadUInt16();
                Recoil = br.ReadUInt16();
                CostPerRound = br.ReadUInt16();
                ShotPrecision = br.ReadUInt16();
                NumberofMagazines = br.ReadUInt16();
                MagazineCapacity = br.ReadUInt16();
                MagazineReloadTime = br.ReadUInt16();
                Weapon_Type = br.ReadEnum16<WeaponType>();
                ChargeTime = br.ReadUInt16();
                KPChargeCost = br.ReadUInt16();
                KojimaMaxDamageRate = br.ReadSingle();
                AttackLatency = br.ReadUInt16();
                Unk3E = br.ReadUInt16();
                Damage_Type = br.ReadEnum8<DamageType>();
                DamagePierce = br.ReadBoolean();
                Unk41 = br.ReadBoolean();
                DamageRadial = br.ReadBoolean();
                AttackPower = br.ReadSingle();
                ImpactForce = br.ReadSingle();
                PAAttentuation = br.ReadSingle();
                PAPenetration = br.ReadSingle();
            }

            /// <summary>
            /// Writes a Projectile component to a stream.
            /// </summary>
            /// <param name="bw">A binary writer.</param>
            public void Write(BinaryWriterEx bw)
            {
                bw.WriteByte((byte)WeaponFiringMode);
                bw.WriteBoolean(CanLockOn);
                bw.WriteByte(MissileLockTime);
                bw.WriteBoolean(Unk03);
                bw.WriteUInt16(FiringRange);
                bw.WriteUInt16(MeleeAbility);
                bw.WriteUInt32(BulletID);
                bw.WriteUInt32(SFXID);
                bw.WriteUInt32(HitEffectID);
                bw.WriteSingle(BallisticsVelocity);
                bw.WriteSingle(ENCost);
                bw.WriteBoolean(MultiProc);
                bw.WriteByte(ProjectileCount);
                bw.WriteByte(ContinuousFireCount);
                bw.WriteByte(Unk1F);
                bw.WriteUInt16(Unk20);
                bw.WriteUInt16(AutoInterval);
                bw.WriteUInt16(FireRate);
                bw.WriteUInt16(Recoil);
                bw.WriteUInt16(CostPerRound);
                bw.WriteUInt16(ShotPrecision);
                bw.WriteUInt16(NumberofMagazines);
                bw.WriteUInt16(MagazineCapacity);
                bw.WriteUInt16(MagazineReloadTime);
                bw.WriteUInt16((ushort)Weapon_Type);
                bw.WriteUInt16(ChargeTime);
                bw.WriteUInt16(KPChargeCost);
                bw.WriteSingle(KojimaMaxDamageRate);
                bw.WriteUInt16(AttackLatency);
                bw.WriteUInt16(Unk3E);
                bw.WriteByte((byte)Damage_Type);
                bw.WriteBoolean(DamagePierce);
                bw.WriteBoolean(Unk41);
                bw.WriteBoolean(DamageRadial);
                bw.WriteSingle(AttackPower);
                bw.WriteSingle(ImpactForce);
                bw.WriteSingle(PAAttentuation);
                bw.WriteSingle(PAPenetration);
            }
        }
    }
}
