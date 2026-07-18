using System.Net;
using System.Text;
using VehicleFinder.Infrastructure.Nhtsa.Services;

namespace VehicleFinder.Tests.Nhtsa
{
    public sealed class NhtsaVehicleServiceTests
    {
        [Fact]
        public async Task GetAllMakesAsync_WhenResponseIsValid_ReturnsMappedMakesOrderedByName()
        {
            const string json = """
            {
              "Count": 2,
              "Message": "Response returned successfully",
              "SearchCriteria": null,
              "Results": [
                {
                  "Make_ID": 448,
                  "Make_Name": "TOYOTA"
                },
                {
                  "Make_ID": 460,
                  "Make_Name": "FORD"
                }
              ]
            }
            """;

            var handler = new StubHttpMessageHandler((request, _) =>
            {
                Assert.Equal(HttpMethod.Get, request.Method);

                Assert.Equal( "https://vpic.test/api/vehicles/GetAllMakes?format=json", request.RequestUri!.AbsoluteUri);

                return Task.FromResult(CreateJsonResponse(json));
            });

            using var httpClient = CreateHttpClient(handler);

            var service = new NhtsaVehicleService(httpClient);

            var result = await service.GetAllMakesAsync();

            Assert.Collection(
                result,
                first =>
                {
                    Assert.Equal(460, first.MakeId);
                    Assert.Equal("FORD", first.MakeName);
                },
                second =>
                {
                    Assert.Equal(448, second.MakeId);
                    Assert.Equal("TOYOTA", second.MakeName);
                });
        }

        [Fact]
        public async Task GetVehicleTypesForMakeAsync_WhenResponseIsValid_ReturnsMappedTypesOrderedByName()
        {
            const string json = """
            {
              "Count": 2,
              "Message": "Response returned successfully",
              "SearchCriteria": "Make ID: 448",
              "Results": [
                {
                  "VehicleTypeId": 7,
                  "VehicleTypeName": "Multipurpose Passenger Vehicle"
                },
                {
                  "VehicleTypeId": 2,
                  "VehicleTypeName": "Passenger Car"
                }
              ]
            }
            """;

            var handler = new StubHttpMessageHandler((request, _) =>
            {
                Assert.Equal( "https://vpic.test/api/vehicles/GetVehicleTypesForMakeId/448?format=json", request.RequestUri!.AbsoluteUri);

                return Task.FromResult(CreateJsonResponse(json));
            });

            using var httpClient = CreateHttpClient(handler);

            var service = new NhtsaVehicleService(httpClient);

            var result = await service.GetVehicleTypesForMakeAsync(448);

            Assert.Collection(
                result,
                first =>
                {
                    Assert.Equal(7, first.VehicleTypeId);
                    Assert.Equal(
                        "Multipurpose Passenger Vehicle",
                        first.VehicleTypeName);
                },
                second =>
                {
                    Assert.Equal(2, second.VehicleTypeId);
                    Assert.Equal(
                        "Passenger Car",
                        second.VehicleTypeName);
                });
        }

        [Fact]
        public async Task GetModelsAsync_WhenResponseIsValid_ReturnsMappedModelsOrderedByName()
        {
            const string json = """
            {
              "Count": 2,
              "Message": "Response returned successfully",
              "SearchCriteria": "Make ID: 474 | Model Year: 2015",
              "Results": [
                {
                  "Make_ID": 474,
                  "Make_Name": "HONDA",
                  "Model_ID": 1861,
                  "Model_Name": "Civic"
                },
                {
                  "Make_ID": 474,
                  "Make_Name": "HONDA",
                  "Model_ID": 1863,
                  "Model_Name": "Accord"
                }
              ]
            }
            """;

            var handler = new StubHttpMessageHandler((request, _) =>
            {
                Assert.Equal(
                    "https://vpic.test/api/vehicles/" +
                    "GetModelsForMakeIdYear/makeId/474/" +
                    "modelyear/2015/vehicletype/Passenger%20Car?format=json",
                    request.RequestUri!.AbsoluteUri);

                return Task.FromResult(CreateJsonResponse(json));
            });

            using var httpClient = CreateHttpClient(handler);

            var service = new NhtsaVehicleService(httpClient);

            var result = await service.GetModelsAsync( 474, 2015, "Passenger Car");

            Assert.Collection(
                result,
                first =>
                {
                    Assert.Equal(474, first.MakeId);
                    Assert.Equal("HONDA", first.MakeName);
                    Assert.Equal(1863, first.ModelId);
                    Assert.Equal("Accord", first.ModelName);
                },
                second =>
                {
                    Assert.Equal(474, second.MakeId);
                    Assert.Equal("HONDA", second.MakeName);
                    Assert.Equal(1861, second.ModelId);
                    Assert.Equal("Civic", second.ModelName);
                });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetVehicleTypesForMakeAsync_WhenMakeIdIsInvalid_ThrowsArgumentOutOfRangeException(int makeId)
        {
            var handler = CreateUnexpectedRequestHandler();

            using var httpClient = CreateHttpClient(handler);

            var service = new NhtsaVehicleService(httpClient);

            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>( () => service.GetVehicleTypesForMakeAsync(makeId));

            Assert.Equal("makeId", exception.ParamName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetModelsAsync_WhenMakeIdIsInvalid_ThrowsArgumentOutOfRangeException(int makeId)
        {
            var handler = CreateUnexpectedRequestHandler();

            using var httpClient = CreateHttpClient(handler);

            var service = new NhtsaVehicleService(httpClient);

            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => service.GetModelsAsync( makeId, 2015, "Passenger Car"));

          
            Assert.Equal("makeId", exception.ParamName);
        }

        [Theory]
        [InlineData(1995)]
        [InlineData(1990)]
        public async Task GetModelsAsync_WhenModelYearIsInvalid_ThrowsArgumentOutOfRangeException(int modelYear)
        {
            var handler = CreateUnexpectedRequestHandler();

            using var httpClient = CreateHttpClient(handler);

            var service = new NhtsaVehicleService(httpClient);

            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => service.GetModelsAsync( 474, modelYear, "Passenger Car"));

            Assert.Equal("modelYear", exception.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetModelsAsync_WhenVehicleTypeIsInvalid_ThrowsArgumentException(string vehicleType)
        {
            
            var handler = CreateUnexpectedRequestHandler();

            using var httpClient = CreateHttpClient(handler);

            var service = new NhtsaVehicleService(httpClient);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => service.GetModelsAsync( 474, 2015,vehicleType));

            Assert.Equal("vehicleType", exception.ParamName);
        }

        [Fact]
        public async Task GetAllMakesAsync_WhenApiReturnsHttpError_ThrowsHttpRequestException()
        {
            var handler = new StubHttpMessageHandler((_, _) =>
            {
                var response = new HttpResponseMessage( HttpStatusCode.ServiceUnavailable);

                return Task.FromResult(response);
            });

            using var httpClient = CreateHttpClient(handler);

            var service = new NhtsaVehicleService(httpClient);

            await Assert.ThrowsAsync<HttpRequestException>( () => service.GetAllMakesAsync());
        }

        [Fact]
        public async Task GetAllMakesAsync_WhenApiReturnsNullBody_ThrowsInvalidOperationException()
        {
            var handler = new StubHttpMessageHandler((_, _) =>
            {
                return Task.FromResult(CreateJsonResponse("null"));
            });

            using var httpClient = CreateHttpClient(handler);

            var service = new NhtsaVehicleService(httpClient);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.GetAllMakesAsync());

           
            Assert.Equal( "NHTSA API returned an empty response.", exception.Message);
        }

        private static HttpClient CreateHttpClient(HttpMessageHandler handler)
        {
            return new HttpClient(handler)
            {
                BaseAddress = new Uri( "https://vpic.test/api/vehicles/")
            };
        }

        private static HttpResponseMessage CreateJsonResponse(string json)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent( json, Encoding.UTF8, "application/json")
            };
        }

        private static StubHttpMessageHandler CreateUnexpectedRequestHandler()
        {
            return new StubHttpMessageHandler((_, _) =>
            {
                throw new InvalidOperationException( "The HTTP API should not be called.");
            });
        }

        private sealed class StubHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage,CancellationToken,Task<HttpResponseMessage>> _handler;

            public StubHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
            {
                _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return _handler(request, cancellationToken);
            }
        }
    }
}
