using System;
using System.Collections.Generic;
using System.Text;
using BangazonAPI.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Xunit;
using System.Net;
using System.Net.Http;


namespace BangazonAPITest
{
    public class ProductTypeTestController
    {
        // Setting a Dummy Product Type to use for Testing 
        private ProductType dummyProductType { get; set; } = new ProductType
        {
            Name = "Test Product",
        };

        //Creating a Private Url for Product Type
        private string url { get; set; } = "api/ProductType";

        //Creating a Dummy Product Type for Testing
        public async Task<ProductType> CreateDummyProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                //Turn the String into Json
                string testingProductTypeAsJson = JsonConvert.SerializeObject(dummyProductType);
                HttpResponseMessage response = await client.PostAsync(
                    url,
                    new StringContent(testingProductTypeAsJson, Encoding.UTF8, "application/json")
                    );

                //Store the Json body of the Response
                string responseBody = await response.Content.ReadAsStringAsync();

                //Turn the information that is Json into an instance of an Product
                ProductType newlyCreatedProductType = JsonConvert.DeserializeObject<ProductType>(responseBody);

                return newlyCreatedProductType;

            }

        }

        // Method for Deleting the Dummy Product Type after Testing
        public async Task deleteDummyProductType(ProductType productTypeToDelete)
        {
            using (HttpClient client = new APIClientProvider().Client)
            {
                HttpResponseMessage deleteResponse = await client.DeleteAsync($"{url}/{productTypeToDelete.Id}");
            }
        }

        [Fact]
        public async Task Create_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {

                //New Testing Project
                ProductType newTestingProductType = await CreateDummyProductType();

                HttpResponseMessage response = await client.GetAsync($"{url}/{newTestingProductType.Id}");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                ProductType newProductType = JsonConvert.DeserializeObject<ProductType>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(dummyProductType.Name, newProductType.Name);

                await deleteDummyProductType(newProductType);

            }
        }

        //Test for Getting all Product Types
        [Fact]
        public async Task Get_All_ProductTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                List<ProductType> productTypes = JsonConvert.DeserializeObject<List<ProductType>>(responseBody);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productTypes.Count > 0);
            }
        }

        //Test For Getting one Product Type by id
        [Fact]
        public async Task Get_One_ProductType()
        {
            using (HttpClient client = new APIClientProvider().Client)
            {
                ProductType newTestingProductType = await CreateDummyProductType();

                HttpResponseMessage response = await client.GetAsync($"{url}/{newTestingProductType.Id}");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                ProductType productTypeFromDB = JsonConvert.DeserializeObject<ProductType>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(dummyProductType.Name, productTypeFromDB.Name);

                await deleteDummyProductType(productTypeFromDB);
            }
        }

        //Test for Updating a Product TYpe
        [Fact]
        public async Task Update_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                ProductType newTestingProductType = await CreateDummyProductType();

                string newName = "Testing Test API";
                newTestingProductType.Name = newName;

                string testingProductTypeAsJson = JsonConvert.SerializeObject(newTestingProductType);

                var response = await client.PutAsync(
                    $"{url}/{newTestingProductType.Id}",
                    new StringContent(testingProductTypeAsJson, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var getModifiedProductType = await client.GetAsync($"{url}/{newTestingProductType.Id}");
                getModifiedProductType.EnsureSuccessStatusCode();

                string getProductTypeBody = await getModifiedProductType.Content.ReadAsStringAsync();

                ProductType newlyEditedProductType = JsonConvert.DeserializeObject<ProductType>(getProductTypeBody);

                Assert.Equal(HttpStatusCode.OK, getModifiedProductType.StatusCode);
                Assert.Equal(newName, newlyEditedProductType.Name);

                await deleteDummyProductType(newlyEditedProductType);
            }
        }

        // Test for Deleting A Product Type
        [Fact]
        public async Task Delete_ProductType()
        {
            ProductType newTestingProductType = await CreateDummyProductType();

            await deleteDummyProductType(newTestingProductType);

            using (var client = new APIClientProvider().Client)
            {
                HttpResponseMessage response = await client.GetAsync($"{url}{newTestingProductType.Id}");

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

    }
}
