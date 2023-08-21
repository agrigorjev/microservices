
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Mandara.CalendarsService.GrpcServices;
using Xunit;
using Xunit.Abstractions;
using Mandara.CalendarsService.Services;
using Mandara.CalendarsService.Data;
using Mandara.CalendarsGrpcTests.Service;
using Mandara.CalendarsGrpcTests.Helpers;
using Mandara.CalendarsGrpcTests.IntegrationTests;
using Optional;
using Mandara.ProductService.Data.Entities;
using Mandara.ProductService.Services;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CustomTypes;
using Microsoft.Extensions.Options;
using System;
using Mandara.ProductService.GrpcDefinitions;
using Mandara.CalendarsServiceTests.Client;
using System.Runtime.CompilerServices;
using NuGet.Frameworks;

namespace Tests.Server.IntegrationTests
{
    public class MockedProductServiceTests : IntegrationTestBase
    {

        private static Random r = new Random(DateTime.Now.Millisecond);

        private static List<Product> testData;
        private static List<SecurityDefinition> testDataSecurityDefinitions;

        public MockedProductServiceTests(GrpcTestFixture<Startup> fixture,
            ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            testData = generateList(200);
            testDataSecurityDefinitions = generateListDefinitions(200);

            var mockProducts = new Mock<IProductStorage>();

            mockProducts.Setup(c => c.GetProducts()).Returns(() =>
            {
                return testData;
            });

            mockProducts.Setup(c => c.GetProduct(It.IsAny<int>())).Returns((int i) =>
            {
                var p= testData.FirstOrDefault(sc => sc.ProductId == i);
                return p==null ? Optional.Option.None<Product>() : Optional.Option.Some(p);
            });

            mockProducts.Setup(c => c.GetSecurityDefinitions()).Returns(() =>
            {
                return testDataSecurityDefinitions;
            });

            mockProducts.Setup(c => c.GetSecurityDefinition(It.IsAny<int>())).Returns((int i) =>
            {
                var p = testDataSecurityDefinitions.FirstOrDefault(sc => sc.SecurityDefinitionId == i);
                return p == null ? Optional.Option.None<SecurityDefinition>() : Optional.Option.Some(p);
            });

            Fixture.ConfigureWebHost(builder =>
            {
                builder.ConfigureServices(
                    services => services.AddSingleton(mockProducts.Object));
            });

        }

        [Fact]
        public void AllProducts_Success()
        {
            // Arrange
            var client = new ProductServicesClient(Channel);

            // Act
           var productResponse
                = client.GetAllProducts(new GetAllRequestMessage());

            // Assert
            Assert.NotEmpty(productResponse.Products);
            Assert.Equal(testData.Count, productResponse.Products.Count);
            int rndId= r.Next(0, testData.Count - 1);
            Assert.True(productEquals(testData[rndId], productResponse.Products[rndId]));
            rndId = r.Next(0, testData.Count - 1);
            Assert.True(productEquals(testData[rndId], productResponse.Products[rndId]));
        }

        [Fact]
        public void SingleProduct_Success()
        {
            // Arrange
            var client = new ProductServicesClient(Channel);

            // Act
            int checkId = r.Next(0, testData.Count -1);
            var response = client.GetProduct(new GetByIdRequestMessage() { Id = testData[checkId].ProductId });

            // Assert
            Assert.NotNull(response.Product);
            Assert.True(productEquals(testData[checkId], response.Product));
        }
        [Fact]
        public void AllDefinitions_Success()
        {
            // Arrange
            var client = new ProductServicesClient(Channel);

            // Act
            var response
                 = client.GetAllSecurityDefinitions(new GetAllRequestMessage());

            // Assert
            Assert.NotEmpty(response.SecurityDefinitions);
            Assert.Equal(testDataSecurityDefinitions.Count, response.SecurityDefinitions.Count);
            int rndId = r.Next(0, testDataSecurityDefinitions.Count - 1);
            Assert.True(definitionEquals(testDataSecurityDefinitions[rndId], response.SecurityDefinitions[rndId]));
            rndId = r.Next(0, testDataSecurityDefinitions.Count - 1);
            Assert.True(definitionEquals(testDataSecurityDefinitions[rndId], response.SecurityDefinitions[rndId]));
        }

        [Fact]
        public void SingleDefinition_Success()
        {
            // Arrange
            var client = new ProductServicesClient(Channel);

            // Act
            int checkId = r.Next(0, testDataSecurityDefinitions.Count - 1);
            var response = client.GetSecurrityDefinition(new GetByIdRequestMessage() { Id = testData[checkId].ProductId });

            // Assert
            Assert.NotNull(response.SecurityDefinition);
            Assert.True(definitionEquals(testDataSecurityDefinitions[checkId], response.SecurityDefinition));
        }


        private DateTime randomDate(int minOffset)
        {
           
            return DateTime.Now.AddDays(r.Next(minOffset, minOffset+100));
        }


        private DateTime? randomDateNullable(int minOffset)
        {
            return r.Next(100) < 50 ? null:DateTime.Now.AddDays(r.Next(minOffset, minOffset + 100));
        }

        private decimal? randomDecimalNullable()
        {
            return r.Next(100) < 50 ? null :  (decimal)r.NextSingle();
        }

        private int? randomIntNullable(int max)
        {
            return r.Next(100) < 50 ? null : r.Next(max);
        }
        private List<Product> generateList(int howMany)
        {

           return Enumerable.Range(1, howMany).Select
                (x =>
                {
                    return new Product()
                    {
                        ProductId = x,
                        calendar_id = r.Next(),
                        holidays_calendar_id = randomIntNullable(500),
                        Name = "Product_" + x,
                        PositionFactor = randomDecimalNullable(),
                        ValidFrom = randomDateNullable(-100)

                    };
                }).ToList();

        }
        private List<SecurityDefinition> generateListDefinitions(int howMany)
        {

            return Enumerable.Range(1, howMany).Select
                 (x =>
                 {
                     return new SecurityDefinition()
                     {
                         SecurityDefinitionId = x,
                         product_id = r.Next(),
                         UnderlyingSymbol = "Symbol_"+x,
                         UnderlyingContractMultiplier = randomDecimalNullable(),
                        LotSize = randomIntNullable(20),
                        StartDateAsDate = randomDateNullable(-100),
                        EndDateAsDate = randomDateNullable(100),
                     };
                 }).ToList();

        }


        private bool productEquals(Product testDAO, ProductGrpc recievd)
        {
            return
               testDAO.ProductId == recievd.ProductId &&
               testDAO.calendar_id == recievd.CalendarId &&
               testDAO.holidays_calendar_id.Equals(recievd.HolidaysCalendarId) &&
               testDAO.Name.Equals(recievd.Name) &&
               testDAO.PositionFactor.Equals(recievd.PositionFactor!=null?(decimal)recievd.PositionFactor:null) &&
               testDAO.ValidFrom.Equals(recievd.ValidFrom!=null? recievd.ValidFrom.ToDateTime().ToLocalTime():null);

        }
        private bool definitionEquals(SecurityDefinition testDAO, SecurityDefinitionGrpc recievd)
        {
            return
               testDAO.SecurityDefinitionId == recievd.SecurityDefinitionId &&
               testDAO.product_id == recievd.ProductId &&
               testDAO.UnderlyingSymbol.Equals(recievd.UnderlyingSymbol) &&
               testDAO.UnderlyingContractMultiplier.Equals(recievd.UnderlyingContractMultiplier==null?null:(decimal) recievd.UnderlyingContractMultiplier) &&
               testDAO.LotSize.Equals(recievd.LotSize) &&
               testDAO.StartDateAsDate.Equals(recievd.StartDateAsDate!=null?recievd.StartDateAsDate.ToDateTime().ToLocalTime():null) &&
               testDAO.EndDateAsDate.Equals(recievd.EndDateAsDate!=null?recievd.EndDateAsDate.ToDateTime().ToLocalTime():null);
        }


       

    }
}