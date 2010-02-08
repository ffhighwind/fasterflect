﻿#region License

// Copyright 2009 Buu Nguyen (http://www.buunguyen.net/blog)
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://fasterflect.codeplex.com/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fasterflect.Emitter
{
	/// <summary>
	/// Stores all necessary information to construct a dynamic method.
	/// </summary>
	internal class CallInfo
	{
		#region EqualityComparer
		internal class CallInfoComparer : IEqualityComparer<CallInfo>
		{
			#region Implementation of IEqualityComparer<CallInfo>
			public bool Equals( CallInfo x, CallInfo y )
			{
				return x.Equals( y );
			}

			public int GetHashCode( CallInfo obj )
			{
				return obj.hashCode;
			}
			#endregion
		}
		#endregion

		public Type TargetType { get; private set; }
        public MemberTypes MemberTypes { get; set; }
        public Type[] ParamTypes { get; private set; }
        public string Name { get; private set; }
        public bool IsStatic { get; private set; }
		private readonly int hashCode;

		private static readonly CallInfoComparer comparer = new CallInfoComparer();
		
		public static CallInfoComparer Comparer
		{
			get { return comparer; }
		}

		public CallInfo(Type targetType, MemberTypes memberTypes, string name, Type[] paramTypes)
			: this(targetType, memberTypes, name, paramTypes, false)
		{
		}

		public CallInfo(Type targetType, MemberTypes memberTypes, string name, Type[] paramTypes, bool isStatic)
		{
			TargetType = targetType;
			MemberTypes = memberTypes;
			Name = name;
			ParamTypes = paramTypes.Length == 0 ? Type.EmptyTypes : paramTypes;
			IsStatic = isStatic;
			hashCode = CalculateHashCode();
		}

		/// <summary>
		/// The CIL should handle inner struct only when the target type is 
		/// a value type or the wrapper ValueTypeHolder type.  In addition, the call 
		/// must also be executed in the non-static context since static 
		/// context doesn't need to handle inner struct case.
		/// </summary>
		public bool ShouldHandleInnerStruct
		{
			get { return IsTargetTypeStruct && !IsStatic; }
		}

		public bool IsTargetTypeStruct
		{
			get { return TargetType.IsValueType; }
		}

		public bool HasNoParam
		{
			get { return ParamTypes == Type.EmptyTypes; }
		}

		public bool HasRefParam
		{
			get { return ParamTypes.Any(t => t.IsByRef); }
		}

        public BindingFlags ScopeFlag
        {
            get { return IsStatic ? BindingFlags.Static : BindingFlags.Instance; }
        }

		/// <summary>
		/// Two <c>CallInfo</c> instances are considered equaled if the following properties
		/// are equaled: <c>TargetType</c>, <c>MemberTypes</c>, <c>Name</c>,
		/// and <c>ParamTypes</c>.
		/// </summary>
		public override bool Equals(object obj)
		{
			var other = obj as CallInfo;
			if (other == null) return false;
			if (other == this) return true;
			if( hashCode != other.GetHashCode() ) return false;

			if (other.TargetType != TargetType || other.MemberTypes != MemberTypes ||
			    other.Name != Name || other.ParamTypes.Length != ParamTypes.Length)
				return false;
			for (int i = 0; i < ParamTypes.Length; i++)
				if (ParamTypes[i] != other.ParamTypes[i])
					return false;
			return true;
		}

		public override int GetHashCode()
		{
			return hashCode;
		}

		internal int CalculateHashCode()
		{
			// simple hash, 60ms
			//return TargetType.GetHashCode() + (31 * (int) MemberTypes) ^ ParamTypes.Length * Name.GetHashCode();
			
			// decent hash, 100ms
			int hash = TargetType.GetHashCode() * ((31 * (int) MemberTypes) ^ Name.GetHashCode());
			for( int i = 0; i < ParamTypes.Length; i++ )
			    hash += (i + 31) * ParamTypes[i].GetHashCode();
			return hash;
			
			// original hash, 520ms
			//int hash = 7;
			//hash = 31 * hash + TargetType.GetHashCode();
			//hash = 31 * hash + MemberTypes.GetHashCode();
			//hash = 31 * hash + Name.GetHashCode();
			//for (int i = 0; i < ParamTypes.Length; i++)
			//    hash = 31 * hash + ParamTypes[i].GetHashCode();
			//return hash;
		}
	}
}