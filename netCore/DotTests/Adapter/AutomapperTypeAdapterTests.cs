using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dot.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DotTests;
using Dot.Adapter;

namespace Dot.Adapter.Tests
{
    [TestClass()]
    public class AutomapperTypeAdapterTests : TestBase
    {

        [TestMethod()]
        public void AdaptTest()
        {
            DomainModelTest dm = new DomainModelTest();
            dm.Id = 100;
            dm.Name = "TEST";
            DTOModelTest dto = dm.ProjectedAs<DTOModelTest>();
            Assert.IsNotNull(dto);
            Assert.AreEqual(100, dto.Id);
            Assert.AreEqual("TEST", dto.Name);
        }

        [TestMethod()]
        public void AdaptTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AdaptTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetMapTargetTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetMapTargetTest1()
        {
            Assert.Fail();
        }
    }



    class DomainModelTest
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }


    class DTOModelTest
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class DotProfiles : Profile
    {
        public DotProfiles()
        {
            //DataDictTypes => DataDictTypesDto
            var DataDictTypesMappingExpression = CreateMap<DomainModelTest, DTOModelTest>();
            //DataDictTypesMappingExpression.ForMember(dto => dto.DataDictTypeId, mc => mc.MapFrom(e => e.Id));

            //DataDictTypesDto => DataDictTypes
            var DataDictTypesDtoMappingExpression = CreateMap<DTOModelTest, DomainModelTest>();
            //DataDictTypesDtoMappingExpression.ForMember(mc => mc.Id, dto => dto.MapFrom(e => e.DataDictTypeId));

        }
    }
}