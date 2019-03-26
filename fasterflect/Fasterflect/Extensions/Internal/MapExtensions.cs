﻿#region License
// Copyright 2010 Buu Nguyen, Morten Mertner
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
using System.Text;
namespace Fasterflect.Extensions
{
	/// <summary>
	/// Extension methods for mapping (copying) members from one object instance to another.
	/// </summary>
	public static partial class MapExtensions
	{
		#region Map
		/// <summary>
		/// Maps values from fields and properties on the source object to fields and properties with the 
		/// same name on the target object.
		/// </summary>
		/// <param name="source">The source object from which member values are read.</param>
		/// <param name="target">The target object to which member values are written.</param>
		/// <param name="names">The optional list of member names against which to filter the members that are
		/// to be mapped. If this parameter is <c>null</c> or empty no name filtering will be applied. The default 
		/// behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.PartialNameMatch"/> to 
		/// filter members by substring and <see href="Flags.IgnoreCase"/> to ignore case.</param>
		public static void Map(this object source, object target, params string[] names)
		{
			Fasterflect.Extensions.MapExtensions.DelegateForMap(source.GetType(), target.GetTypeAdjusted(), names)(source, target);
		}

		/// <summary>
		/// Maps values from fields and properties on the source object to fields and properties with the 
		/// same name on the target object.
		/// </summary>
		/// <param name="source">The source object from which member values are read.</param>
		/// <param name="target">The target object to which member values are written.</param>
		/// <param name="bindingFlags">The <see href="Flags"/> used to define the scope when locating members.</param>
		/// <param name="names">The optional list of member names against which to filter the members that are
		/// to be mapped. If this parameter is <c>null</c> or empty no name filtering will be applied. The default 
		/// behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.PartialNameMatch"/> to 
		/// filter members by substring and <see href="Flags.IgnoreCase"/> to ignore case.</param>
		public static void Map(this object source, object target, FasterflectFlags bindingFlags, params string[] names)
		{
			Fasterflect.Extensions.MapExtensions.DelegateForMap(source.GetType(), target.GetTypeAdjusted(), bindingFlags, names)(source, target);
		}

		/// <summary>
		/// Maps values from members on the source object to members with the same name on the target object.
		/// </summary>
		/// <param name="source">The source object from which member values are read.</param>
		/// <param name="target">The target object to which member values are written.</param>
		/// <param name="sourceTypes">The member types (Fields, Properties or both) to include on the source.</param>
		/// <param name="targetTypes">The member types (Fields, Properties or both) to include on the target.</param>
		/// <param name="bindingFlags">The <see href="Flags"/> used to define the scope when locating members. If
		/// <paramref name="sourceTypes"/> is different from <paramref name="targetTypes"/> the flag value
		/// <see cref="FasterflectFlags.IgnoreCase"/> will automatically be applied.</param>
		/// <param name="names">The optional list of member names against which to filter the members that are
		/// to be mapped. If this parameter is <c>null</c> or empty no name filtering will be applied. The default 
		/// behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.PartialNameMatch"/> to 
		/// filter members by substring and <see href="Flags.IgnoreCase"/> to ignore case.</param>
		public static void Map(this object source, object target, MemberTypes sourceTypes, MemberTypes targetTypes,
								FasterflectFlags bindingFlags, params string[] names)
		{
			Fasterflect.Extensions.MapExtensions.DelegateForMap(source.GetType(), target.GetTypeAdjusted(), sourceTypes, targetTypes, bindingFlags, names)(
				source, target);
		}
		#endregion

		#region Map Companions
		/// <summary>
		/// Maps values from fields on the source object to fields with the same name on the target object.
		/// </summary>
		/// <param name="source">The source object from which field values are read.</param>
		/// <param name="target">The target object to which field values are written.</param>
		/// <param name="names">The optional list of field names against which to filter the fields that are
		/// to be mapped. If this parameter is <c>null</c> or empty no name filtering will be applied. The default 
		/// behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.PartialNameMatch"/> to 
		/// filter fields by substring and <see href="Flags.IgnoreCase"/> to ignore case.</param>
		public static void MapFields(this object source, object target, params string[] names)
		{
			source.Map(target, MemberTypes.Field, MemberTypes.Field, FasterflectFlags.InstanceAnyVisibility, names);
		}

		/// <summary>
		/// Maps values from properties on the source object to properties with the same name on the target object.
		/// </summary>
		/// <param name="source">The source object from which property values are read.</param>
		/// <param name="target">The target object to which property values are written.</param>
		/// <param name="names">The optional list of property names against which to filter the properties that are
		/// to be mapped. If this parameter is <c>null</c> or empty no name filtering will be applied. The default 
		/// behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.PartialNameMatch"/> to 
		/// filter properties by substring and <see href="Flags.IgnoreCase"/> to ignore case.</param>
		public static void MapProperties(this object source, object target, params string[] names)
		{
			source.Map(target, MemberTypes.Property, MemberTypes.Property, FasterflectFlags.InstanceAnyVisibility, names);
		}

		/// <summary>
		/// Maps values from fields on the source object to properties with the same name (ignoring case)
		/// on the target object.
		/// </summary>
		/// <param name="source">The source object from which field values are read.</param>
		/// <param name="target">The target object to which property values are written.</param>
		/// <param name="names">The optional list of member names against which to filter the members that are
		/// to be mapped. If this parameter is <c>null</c> or empty no name filtering will be applied. The default 
		/// behavior is to check for an exact, case-insensitive match. Pass <see href="Flags.PartialNameMatch"/>
		/// to filter members by substring.</param>
		public static void MapFieldsToProperties(this object source, object target, params string[] names)
		{
			source.Map(target, MemberTypes.Field, MemberTypes.Property, FasterflectFlags.InstanceAnyVisibility, names);
		}

		/// <summary>
		/// Maps values from properties on the source object to fields with the same name (ignoring case) 
		/// on the target object.
		/// </summary>
		/// <param name="source">The source object from which property values are read.</param>
		/// <param name="target">The target object to which field values are written.</param>
		/// <param name="names">The optional list of member names against which to filter the members that are
		/// to be mapped. If this parameter is <c>null</c> or empty no name filtering will be applied. The default 
		/// behavior is to check for an exact, case-insensitive match. Pass <see href="Flags.PartialNameMatch"/>
		/// to filter members by substring.</param>
		public static void MapPropertiesToFields(this object source, object target, params string[] names)
		{
			source.Map(target, MemberTypes.Property, MemberTypes.Field, FasterflectFlags.InstanceAnyVisibility, names);
		}
		#endregion
	}
}