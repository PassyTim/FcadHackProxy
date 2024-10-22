﻿using Newtonsoft.Json.Linq;

namespace FcadHackProxy.Data;

public interface IMessageRepository
{
    public Task SaveAsync(JObject jsonMessage);
    public Task<JObject> GetAsync(string id);
    public Task<IEnumerable<JObject>> GetAllAsync(int pageSize, int pageNumber);
    public Task DeleteAsync(string id);
}