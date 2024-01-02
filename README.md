Za pokretanje aplikacije neophodno je:
  - dotnet 8.0 (Za pokretanje API)
  - NRedisStack zavisnost za Redis dotnet client.
  - wsl sa instaliranim redisom 
  - live server ekstenzija za Visual Studio COde (za hostovanje frontend aplikacije)

Pokretanje:
- U repozitorijumu pokrenuti u terminalu komandu "dotnet watch run" za startovanje API-a
- Pokrenuti wsl i u njemu izvrsiti komandu "sudo service redis-server start"
- Pokrenuti live server
               
