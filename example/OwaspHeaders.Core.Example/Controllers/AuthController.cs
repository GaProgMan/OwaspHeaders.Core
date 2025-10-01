using Microsoft.AspNetCore.Mvc;

namespace OwaspHeaders.Core.Example.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Logout endpoint that demonstrates Clear-Site-Data header functionality.
        /// This endpoint clears all client-side data (cache, cookies, storage) for maximum security.
        /// When Clear-Site-Data is configured for this path, check the response headers to see the header in action.
        /// </summary>
        [HttpPost("logout")]
        public ActionResult<object> Logout()
        {
            _logger.LogInformation("Processing logout request - Clear-Site-Data header will be applied if configured");

            var response = new
            {
                message = "Logout successful",
                clearSiteData = new
                {
                    note = "If Clear-Site-Data is configured for /auth/logout, check response headers",
                    expectedHeader = "Clear-Site-Data: \"*\"",
                    description = "This header instructs the browser to clear all cached data, cookies, and storage for this origin"
                },
                headers = GetHeaders,
                securityInfo = new
                {
                    benefit = "Clear-Site-Data helps prevent session hijacking by ensuring complete logout",
                    recommendation = "Use wildcard (*) for logout endpoints to clear all client-side data"
                }
            };

            return Ok(response);
        }

        /// <summary>
        /// Alternative logout endpoint for API authentication.
        /// This demonstrates selective data clearing (cache and cookies only, preserving storage).
        /// </summary>
        [HttpPost("api/logout")]
        public ActionResult<object> ApiLogout()
        {
            _logger.LogInformation("Processing API logout request - Selective Clear-Site-Data will be applied if configured");

            var response = new
            {
                message = "API logout successful",
                clearSiteData = new
                {
                    note = "If Clear-Site-Data is configured for /auth/api/logout, check response headers",
                    expectedHeader = "Clear-Site-Data: \"cache\",\"cookies\"",
                    description = "This header clears cache and cookies while preserving localStorage/sessionStorage"
                },
                headers = GetHeaders,
                useCase = new
                {
                    scenario = "API logout with selective data clearing",
                    rationale = "Preserves user preferences stored in localStorage while clearing authentication data"
                }
            };

            return Ok(response);
        }

        /// <summary>
        /// Admin logout endpoint for administrative users.
        /// Demonstrates maximum security Clear-Site-Data configuration for privileged accounts.
        /// </summary>
        [HttpPost("admin/logout")]
        public ActionResult<object> AdminLogout()
        {
            _logger.LogInformation("Processing admin logout request - Maximum security Clear-Site-Data will be applied if configured");

            var response = new
            {
                message = "Admin logout successful",
                clearSiteData = new
                {
                    note = "If Clear-Site-Data is configured for /auth/admin/logout, check response headers",
                    expectedHeader = "Clear-Site-Data: \"*\"",
                    description = "Admin logout uses wildcard to ensure complete data clearing for security"
                },
                headers = GetHeaders,
                securityLevel = new
                {
                    level = "Maximum",
                    rationale = "Administrative accounts require complete data clearing to prevent privilege escalation attacks"
                }
            };

            return Ok(response);
        }

        /// <summary>
        /// Login endpoint (no Clear-Site-Data expected).
        /// This demonstrates that Clear-Site-Data is path-specific and only applied to configured logout endpoints.
        /// </summary>
        [HttpPost("login")]
        public ActionResult<object> Login([FromBody] object loginRequest)
        {
            _logger.LogInformation("Processing login request - No Clear-Site-Data header expected");

            var response = new
            {
                message = "Login successful",
                clearSiteData = new
                {
                    note = "No Clear-Site-Data header should be present for login endpoints",
                    reason = "Clear-Site-Data is typically only configured for logout endpoints"
                },
                headers = GetHeaders,
                nextSteps = new
                {
                    logout = "POST /auth/logout - Demonstrates wildcard Clear-Site-Data",
                    apiLogout = "POST /auth/api/logout - Demonstrates selective Clear-Site-Data",
                    adminLogout = "POST /auth/admin/logout - Demonstrates admin-level Clear-Site-Data"
                }
            };

            return Ok(response);
        }

        /// <summary>
        /// Information endpoint that explains Clear-Site-Data configuration options
        /// </summary>
        [HttpGet("info")]
        public ActionResult<object> GetAuthInfo()
        {
            _logger.LogInformation("Providing information about Clear-Site-Data authentication endpoints");

            var response = new
            {
                clearSiteDataDemo = new
                {
                    description = "This controller demonstrates Clear-Site-Data header functionality",
                    endpoints = new
                    {
                        logout = new
                        {
                            path = "POST /auth/logout",
                            clearSiteData = "wildcard (*) - clears all data",
                            useCase = "Standard user logout"
                        },
                        apiLogout = new
                        {
                            path = "POST /auth/api/logout",
                            clearSiteData = "cache,cookies - preserves storage",
                            useCase = "API logout with selective clearing"
                        },
                        adminLogout = new
                        {
                            path = "POST /auth/admin/logout",
                            clearSiteData = "wildcard (*) - maximum security",
                            useCase = "Administrative logout"
                        },
                        login = new
                        {
                            path = "POST /auth/login",
                            clearSiteData = "none - no clearing on login",
                            useCase = "Login (no data clearing needed)"
                        }
                    }
                },
                configuration = new
                {
                    note = "To see Clear-Site-Data in action, uncomment Example 4 in Program.cs",
                    example = "Clear-Site-Data configuration with path-specific rules"
                },
                testing = new
                {
                    recommendation = "Use browser dev tools Network tab to inspect response headers",
                    lookFor = "Clear-Site-Data header in logout endpoint responses"
                }
            };

            return Ok(response);
        }

        private IEnumerable<string> GetHeaders =>
            HttpContext.Response.Headers.Select(h => $"{h.Key}: {h.Value}").ToArray();
    }
}
