using Blazored.LocalStorage;
using TourismFrontend.Models;

namespace TourismFrontend.Services
{
    public class VacationPlanService
    {
        private readonly ILocalStorageService _localStorage;
        private const string STORAGE_KEY = "vacation_plan";

        public event Action? OnPlanChanged;

        public VacationPlanService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<VacationPlan> GetPlanAsync()
        {
            return await _localStorage.GetItemAsync<VacationPlan>(STORAGE_KEY) 
                   ?? new VacationPlan();
        }

        public async Task<ApiResponse<bool>> AddToVacationPlanAsync(Tour tour, RoomType roomType, int numberOfPeople)
        {
            try
            {
                await AddTourAsync(tour, roomType, numberOfPeople);
                return new ApiResponse<bool> { IsSuccess = true, Data = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { IsSuccess = false, Error = ex.Message };
            }
        }

        public async Task AddTourAsync(Tour tour, RoomType roomType, int numberOfPeople)
        {
            var plan = await GetPlanAsync();
            
            var item = new VacationPlanItem
            {
                TourId = tour.Id,
                Tour = tour,
                RoomTypeId = roomType.Id,
                RoomType = roomType,
                NumberOfPeople = numberOfPeople
            };
            
            plan.Items.Add(item);
            
            await _localStorage.SetItemAsync(STORAGE_KEY, plan);
            OnPlanChanged?.Invoke();
        }

        public async Task RemoveItemAsync(int tourId)
        {
            var plan = await GetPlanAsync();
            plan.Items.RemoveAll(i => i.TourId == tourId);
            await _localStorage.SetItemAsync(STORAGE_KEY, plan);
            OnPlanChanged?.Invoke();
        }

        public async Task UpdateItemPeopleCountAsync(int tourId, int count)
        {
            var plan = await GetPlanAsync();
            var item = plan.Items.FirstOrDefault(i => i.TourId == tourId);
            if (item != null)
            {
                item.NumberOfPeople = count;
                await SavePlanAsync(plan);
            }
        }

        public async Task ClearPlanAsync()
        {
            var plan = new VacationPlan
            {
                Items = new List<VacationPlanItem>()
            };
            await SavePlanAsync(plan);
        }

        private async Task SavePlanAsync(VacationPlan plan)
        {
            await _localStorage.SetItemAsync(STORAGE_KEY, plan);
            OnPlanChanged?.Invoke();
        }
    }
} 