MVCScopedClient
===============

OAuth 2.0 clients that support custom scope

How to use
===============
1. Create new MVC 4 web application
2. Add reference to the library
3. Register Client on AuthConfig.cs 
4. OAuthWebSecurity.RegisterClient(new FacebookScopedClient("", "", "PERMISSIONS"), "Facebook", new Dictionary<string, object>());
5. It might give you red underlinin under this but don't care about that and just continue
6. ???
6. Profit
