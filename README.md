MVCScopedClient
===============

OAuth 2.0 clients that support custom scope

How to use
===============
1. Create new MVC 4 web application
2. Add reference to the library
3. Register Client on auth.config 
4. OAuthWebSecurity.RegisterClient(new FacebookScopedClient("", "", "PERMISSIONS"), "Facebook", new Dictionary<string, object>());
5. ???
6. Profit
