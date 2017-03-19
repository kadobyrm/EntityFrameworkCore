﻿using System;

namespace Blueshift.EntityFrameworkCore.MongoDB.Tests.TestDomain
{
    public class DerivedType1 : RootType, IEquatable<DerivedType1>
    {
        public int IntProperty { get; set; } = new Random().Next();

        public override bool Equals(RootType other)
            => Equals(other as DerivedType1);

        public bool Equals(DerivedType1 other)
            => base.Equals(other) &&
               IntProperty == other?.IntProperty;
    }
}