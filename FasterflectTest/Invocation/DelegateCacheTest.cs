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

using Fasterflect;
using Fasterflect.Emitter;
using Fasterflect.Extensions;
using FasterflectTest.SampleModel.People;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Reflection;

namespace FasterflectTest.Invocation
{
	[TestClass]
	public class DelegateCacheTest : BaseInvocationTest
	{
		private readonly object[] objectTypes = {
													typeof(Person).CreateInstance(),
													typeof(PersonStruct).CreateInstance().WrapIfValueType()
												};

		IDictionary Getters;
		IDictionary Setters;
		IDictionary Constructors;
		IDictionary Methods;
		IDictionary ArraySetters;
		IDictionary ArrayGetters;
		IDictionary MultiSetters;
		IDictionary Mappers;

		IDictionary[] Dictionaries;

		[TestInitialize]
		public void TestInitialize()
		{
			Getters = (IDictionary)typeof(Reflect).GetFieldValue("Getters").GetFieldValue("entries");
			Setters = (IDictionary)typeof(Reflect).GetFieldValue("Setters").GetFieldValue("entries");
			Constructors = (IDictionary)typeof(Reflect).GetFieldValue("Constructors").GetFieldValue("entries");
			Methods = (IDictionary)typeof(Reflect).GetFieldValue("Methods").GetFieldValue("entries");
			ArraySetters = (IDictionary)typeof(Reflect).GetFieldValue("ArraySetters").GetFieldValue("entries");
			ArrayGetters = (IDictionary)typeof(Reflect).GetFieldValue("ArrayGetters").GetFieldValue("entries");
			MultiSetters = (IDictionary)typeof(Reflect).GetFieldValue("MultiSetters").GetFieldValue("entries");
			Mappers = (IDictionary)typeof(Reflect).GetFieldValue("Mappers").GetFieldValue("entries");

			Dictionaries = new IDictionary[] {
				Getters,
				Setters,
				Constructors,
				Methods,
				ArraySetters,
				ArrayGetters,
				MultiSetters,
				Mappers,
			};
			for (int i = 0; i < Dictionaries.Length; ++i) {
				Dictionaries[i].Clear();
			}
		}

		private void ExecuteCacheTest(IDictionary[] dicts, params Action[] actions)
		{
			for (int j = 0; j < actions.Length; j++) {
				int delCount = dicts[j].Count;
				Action action = actions[j];
				for (int i = 0; i < 20; i++) {
					action();
				}
				Assert.AreEqual(++delCount, dicts[j].Count);
			}
		}

		[TestMethod]
		public void TestDelegateIsProperlyCachedForFields()
		{
			objectTypes.ForEach(obj => ExecuteCacheTest(new IDictionary[] { Setters, Getters },
													() => obj.SetFieldValue("name", "John"),
													() => obj.GetFieldValue("age")));
			Types.ForEach(type => ExecuteCacheTest(new IDictionary[] { Setters, Getters },
													() => type.SetFieldValue("totalPeopleCreated", 1),
													() => type.GetFieldValue("totalPeopleCreated")));
		}

		[TestMethod]
		public void TestDelegateIsProperlyCachedForProperties()
		{
			objectTypes.ForEach(obj => ExecuteCacheTest(new IDictionary[] { Setters, Getters },
													() => obj.SetPropertyValue("Name", "John"),
													() => obj.GetPropertyValue("Age")));
			Types.ForEach(type => ExecuteCacheTest(new IDictionary[] { Setters, Getters },
													() => type.SetPropertyValue("TotalPeopleCreated", 1),
													() => type.GetPropertyValue("TotalPeopleCreated")));
		}

		[TestMethod]
		public void TestDelegateIsProperlyCachedForConstructors()
		{
			Types.ForEach(type => ExecuteCacheTest(new IDictionary[] { Constructors, Constructors },
												   () => type.CreateInstance("John", 10)));
		}


		[TestMethod]
		public void TestDelegateIsProperlyCachedForConstructors2()
		{
			Types.ForEach(type => ExecuteCacheTest(new IDictionary[] { Constructors },
												   () => type.CreateInstance()));
		}

		[TestMethod]
		public void TestDelegateIsProperlyCachedForMethods()
		{
			object[] arguments = new object[] { 100d, null };
			objectTypes.ForEach(obj => ExecuteCacheTest(new IDictionary[] { Methods, Methods },
								   () => obj.CallMethod("Walk", 100d),
								   () => obj.CallMethod("Walk", new[] { typeof(double), typeof(double).MakeByRefType() }, arguments)));
			Types.ForEach(type => ExecuteCacheTest(new IDictionary[] { Methods, Methods },
													  () => type.CallMethod("GetTotalPeopleCreated"),
													  () => type.CallMethod("AdjustTotalPeopleCreated", 10)));
		}

		[TestMethod]
		public void TestDelegateIsProperlyCachedForIndexers()
		{
			for (int i = 0; i < Types.Length; i++) {
				object emptyDictionary = Types[i].Field("friends").FieldType.CreateInstance();
				objectTypes[i].SetFieldValue("friends", emptyDictionary);
				ExecuteCacheTest(new IDictionary[] { Methods, Methods, Methods },
								() => objectTypes[i].SetIndexer(new[] 
																{
																	typeof(string),
																	Types[i] == typeof(Person) ? typeof(Person) : typeof(PersonStruct?)
																},
																"John", null),
								() => objectTypes[i].GetIndexer("John"),
								() => objectTypes[i].GetIndexer("John", 10));
			}
		}
	}
}