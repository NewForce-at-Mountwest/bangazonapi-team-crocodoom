using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using BangazonAPI.Models;

namespace BangazonAPITest
{
    public class ProductTestController
    {
        // Creating a Dummy Product to Use While Testing
        private Product dummyProduct { get; set; } = new Product
        {
            ProductTypeId = 1,
            CustomerId = 1,
            Price = 3,
            Title = "Dummy Product",
            Description = "This is a Dummy Description",
            Quantity = 4
        };

        private string url { get; set; } = "api/Product";

        //Method for Creating a Dummy Product
        public async Task<Product> CreateDummyProduct()
        {
            using (var client = new APIClientProvider().Client)
            {
                //Turn the String into Json
                string testingProductAsJson = JsonConvert.SerializeObject(dummyProduct);
                HttpResponseMessage response = await client.PostAsync(
                    url,
                    new StringContent(testingProductAsJson, Encoding.UTF8, "application/json")
                    );

                //Store the Json body of the Response
                string responseBody = await response.Content.ReadAsStringAsync();

                //Turn the information that is Json into an instance of an Product
                Product newlyCreatedProduct = JsonConvert.DeserializeObject<Product>(responseBody);

                return newlyCreatedProduct;

            }

        }

        //Method to Delete the Dummy Product After its created 
        public async Task deleteDummyProduct(Product productToDelete)
        {
            using (HttpClient client = new APIClientProvider().Client)
            {
                HttpResponseMessage deleteResponse = await client.DeleteAsync($"{url}/{productToDelete.Id}");
            }
        }

        //Test for Creating a Product
        [Fact]
        public async Task Create_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                
                //New Testing Project
                Product newTestingProduct = await CreateDummyProduct();

                HttpResponseMessage response = await client.GetAsync($"{url}/{newTestingProduct.Id}");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                Product newProduct = JsonConvert.DeserializeObject<Product>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(dummyProduct.ProductTypeId, newProduct.ProductTypeId);
                Assert.Equal(dummyProduct.CustomerId, newProduct.CustomerId);
                Assert.Equal(dummyProduct.Price, newProduct.Price);
                Assert.Equal(dummyProduct.Title, newProduct.Title);
                Assert.Equal(dummyProduct.Description, newProduct.Description);
                Assert.Equal(dummyProduct.Quantity, newProduct.Quantity);

                await deleteDummyProduct(newProduct);
              
            }
        }

        //Test For Getting all Products
        [Fact]
        public async Task Get_All_Products()
        {
            using (var client = new APIClientProvider().Client)
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                List<Product> products = JsonConvert.DeserializeObject<List<Product>>(responseBody);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(products.Count > 0);
            }
        }

        //Test for getting a single Product
        [Fact]
        public async Task Get_Single_Product()
        {
            using (HttpClient client = new APIClientProvider().Client)
            {
                Product newTestingProduct = await CreateDummyProduct();

                HttpResponseMessage response = await client.GetAsync($"{url}/{newTestingProduct.Id}");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                Product productFromDB = JsonConvert.DeserializeObject<Product>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(dummyProduct.ProductTypeId, productFromDB.ProductTypeId);
                Assert.Equal(dummyProduct.CustomerId, productFromDB.CustomerId);
                Assert.Equal(dummyProduct.Price, productFromDB.Price);
                Assert.Equal(dummyProduct.Description, productFromDB.Description);
                Assert.Equal(dummyProduct.Quantity, productFromDB.Quantity);

                await deleteDummyProduct(productFromDB);
            }
        }

        //Test to Update Product
        [Fact]
        public async Task Update_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                Product newTestingProduct = await CreateDummyProduct();

                string newTitle = "Testing Test API";
                newTestingProduct.Title = newTitle;

                string testingProductAsJson = JsonConvert.SerializeObject(newTestingProduct);

                var response = await client.PutAsync(
                    $"{url}/{newTestingProduct.Id}",
                    new StringContent(testingProductAsJson, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var getModifiedProduct = await client.GetAsync($"{url}/{newTestingProduct.Id}");
                getModifiedProduct.EnsureSuccessStatusCode();

                string getProductBody = await getModifiedProduct.Content.ReadAsStringAsync();

                Product newlyEditedProduct = JsonConvert.DeserializeObject<Product>(getProductBody);

                Assert.Equal(HttpStatusCode.OK, getModifiedProduct.StatusCode);
                Assert.Equal(newTitle, newlyEditedProduct.Title);

                await deleteDummyProduct(newlyEditedProduct);
            }
        }

        //Test to Delete a Product
        [Fact]
        public async Task Delete_Product()
        {
            Product newTestingProduct = await CreateDummyProduct();

            await deleteDummyProduct(newTestingProduct);

            using (var client = new APIClientProvider().Client)
            {
                HttpResponseMessage response = await client.GetAsync($"{url}{newTestingProduct.Id}");

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

    }
}
