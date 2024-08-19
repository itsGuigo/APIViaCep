using ApiViaCep.Classes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Net.Http.Headers;

public class Program
{   
    static void Main(string[] args)
    {
        Console.WriteLine("Digite seu CEP:");
        var cep = Console.ReadLine();
        if (!ValidaCep(cep))
        {
            Console.WriteLine("CEP inválido, tente novamente");
        }
        else
        {
            ViaCepModel responseCepHttpClient = APIViaCepHttpClient(cep);
            ViaCepModel responseCepRestSharp = APIViaCepRestSharp(cep);
            if (responseCepHttpClient.Erro != null || responseCepRestSharp.Erro != null)
            {
                Console.WriteLine($"{responseCepHttpClient.Erro}");
            }
            else
            {
                Console.WriteLine($"As informações do seu CEP abaixo HTPPClient:\n" +
                          $"CEP: {responseCepHttpClient.CEP}\n" +
                          $"Rua: {responseCepHttpClient.Logradouro}\n" +
                          $"Bairro: {responseCepHttpClient.Bairro}\n" +
                          $"Cidade: {responseCepHttpClient.Localidade}");

                Console.WriteLine($"\nAs informações do seu CEP abaixo RestSharp:\n" +
                          $"CEP: {responseCepRestSharp.CEP}\n" +
                          $"Rua: {responseCepRestSharp.Logradouro}\n" +
                          $"Bairro: {responseCepRestSharp.Bairro}\n" +
                          $"Cidade: {responseCepRestSharp.Localidade}");
            }
        }
    }

    public static bool ValidaCep(string cep) => cep.All(char.IsDigit);

    [HttpGet]
    public static ViaCepModel APIViaCepHttpClient(string cep)
    {
        ViaCepModel viaCep = new ViaCepModel();
        var client = new HttpClient();
        client.BaseAddress = new Uri("https://viacep.com.br/ws/");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        try
        {
            var request = client.GetAsync($"{cep}/json").Result;
            if (request.IsSuccessStatusCode)
            {
                var response = request.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<ViaCepModel>(response);
            }
            else
            {
                viaCep.AddError($"Erro ao consultar o seu CEP: {cep}, tente novamente");
            }
        }
        catch (Exception e)
        {
            viaCep.AddError($"Erro ao consultar API: {e.Message}");
        }
        return viaCep;
    }

    [HttpGet]
    public static ViaCepModel APIViaCepRestSharp(string cep)
    {
        ViaCepModel viaCep = new ViaCepModel();
        var client = new RestClient("https://viacep.com.br/ws/");
        var endpoint = $"{cep}/json";
        var request = new RestRequest(endpoint, Method.Get);
        request.AddHeader("Content-Type", "application/json");
        try
        {
            var response = client.Execute(request);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ViaCepModel>(response.Content);
            }
            else
            {
                viaCep.AddError(response.ErrorMessage == null ? $"Erro ao consultar seu CEP: {cep}, tente novamente" : response.ErrorMessage);
            }
        }
        catch (Exception e)
        {
            viaCep.AddError($"Erro ao consultar API: {e.Message}");
        }
        
        return viaCep;
    }
}