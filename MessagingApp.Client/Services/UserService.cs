using MessagingApp.Client.Models.User;
using System.Net;
using System.Net.Http.Headers;

namespace MessagingApp.Client.Services
{
    public class UserService
    {
        private readonly HttpClient _http;
        private readonly CookieService _cookieService;

        public UserService(IHttpClientFactory factory, CookieService cookieService)
        {
            _http = factory.CreateClient("Api"); // ✅ IMPORTANT
            _cookieService = cookieService;
        }

        public async Task<List<UserRequest>> GetUsersAsync()
        {
            try
            {
                var response = await _http.GetAsync("/api/User/get-users");
                
                Console.WriteLine($"[UserService] Status Code: {response.StatusCode}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("[UserService] Unauthorized - user not logged in");
                    return new List<UserRequest>();
                }
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[UserService] Error Response: {errorBody}");
                    return new List<UserRequest>();
                }
                
                var users = await response.Content.ReadFromJsonAsync<List<UserRequest>>();

                return users ?? new List<UserRequest>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[UserService] Network error: {ex.Message}");
                return new List<UserRequest>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Unexpected error: {ex}");
                return new List<UserRequest>();
            }
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            try
            {
                var token = await _cookieService.GetToken();

                if (string.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine("No token found");
                    return false;
                }

                _http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer", token);
                
                var response = await _http.DeleteAsync($"/api/Auth/delete/{userId}");

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("Unauthorized");
                    return false;
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Delete failed: {error}");
                    return false;
                }

                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Network error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return false;
            }
        }

        public async Task<UpdateUserRequest?> GetCurrentUserAsync(Guid id)
        {
            var token = await _cookieService.GetToken();

            Console.WriteLine($"TOKEN FOUND: {!string.IsNullOrEmpty(token)}");

            if (string.IsNullOrWhiteSpace(token))
                return null;

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.GetAsync($"/api/User/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<UpdateUserRequest>();
        }


        public async Task<bool> EditUserAsync(UpdateUserRequest updatedUser)
        {
            try
            {
                // Get JWT token from cookies
                var token = await _cookieService.GetToken();

                if (string.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine("No token found");
                    return false;
                }

                // Add Authorization header
                _http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // Send PUT request to update the user
                var response = await _http.PutAsJsonAsync($"/api/Auth/edit/{updatedUser.userId}", updatedUser);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"EditUser failed: {error}");

                    // Optional: handle unauthorized specifically
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        Console.WriteLine("User is not authorized to edit this user");
                    }

                    return false;
                }

                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Network error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> AssignRoleAsync(RoleRequest request)
        {
            try
            {
                var token = await _cookieService.GetToken();

                if (string.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine("No token found");
                    return false;
                }

                _http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer", token);

                var response = await _http.PostAsJsonAsync(
                    "/api/User/assign-role",
                    request
                );

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("[UserService] Unauthorized - user not logged in");                    
                }

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    Console.WriteLine("[UserService] Forbidden - insufficient permissions");                    
                }


                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"AssignRole failed: {error}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AssignPermissionAsync(PermissionsRequest request)
        {
            try
            {
                var token = await _cookieService.GetToken();

                if (string.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine("No token found");
                    return false;
                }

                _http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer", token);

                var response = await _http.PostAsJsonAsync(
                    "/api/User/assign-permissions",
                    request
                );

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"AssignPermission failed: {error}");
                    
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemovePermissionAsync(PermissionsRequest request)
        {
            try
            {
                var token = await _cookieService.GetToken();

                if (string.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine("No token found");
                    return false;
                }

                _http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer", token);

                var response = await _http.PostAsJsonAsync(
                    "/api/User/remove-permissions",
                    request
                );

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"RemovePermission failed: {error}");

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return false;
            }
        }

    }
}
