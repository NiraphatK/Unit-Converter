using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace Mid_Project
{
    public partial class HistoryPage : ContentPage
    {
        private readonly ConversionService _conversionService;
        public ObservableCollection<ConversionHistory> HistoryList { get; set; }
        private DateTime _lastFileWriteTime; // �����ҷ�����١��¹�����ش����
        private bool _isDataLoaded; 

        public HistoryPage()
        {
            InitializeComponent();
            _conversionService = new ConversionService();
            HistoryList = new ObservableCollection<ConversionHistory>();
            BindingContext = this; // ������§ BindingContext
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // �ʴ� UI ��������͹ ���Ǥ�����Ŵ������
            await Task.Delay(100); // ˹�ǧ������� � �������˹���ʴ���

            // ����ѧ�������Ŵ������ �����ա������¹�ŧ����
            if (!_isDataLoaded)
            {
                await LoadHistoryAsync();
                _isDataLoaded = true; // ��駤��ʶҹ������Ŵ����������
            }
            else
            {
                // ��Ǩ�ͺ�������ա����������������
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "conversion_history.json");
                DateTime lastWriteTime = File.Exists(filePath) ? File.GetLastWriteTime(filePath) : DateTime.MinValue;

                // �ҡ����ա��������������Ŵ����������
                if (lastWriteTime > _lastFileWriteTime)
                {
                    await LoadHistoryAsync();
                }

                _lastFileWriteTime = lastWriteTime; // �ѻവ������¹���
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(); // ����� UI �����Ҥ������¹
            }
        }
        // �ѧ��ѹ����Ѻ��Ŵ�����Ż���ѵԡ���ŧ�ҡ���Ẻ Asynchronous
        public async Task LoadHistoryAsync()
        {
            IsLoading = true; // ������ʴ� ActivityIndicator  

            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "conversion_history.json");

                // ��Ǩ�ͺ�������բ������������
                if (File.Exists(filePath))
                {
                    // ���� Delay ��硹������ͨ��ͧ�����Ŵ������
                    await Task.Delay(100);

                    // �� Task.Run ������Ŵ������� background thread
                    var historyList = await Task.Run(() =>
                    {
                        return _conversionService.LoadHistoryFromFile();
                    });

                    if (historyList != null && historyList.Count > 0)
                    {
                        Dispatcher.Dispatch(() =>
                        {
                            // ��������������� ObservableCollection
                            HistoryList.Clear();

                            // ���§�ӴѺ�����Ũҡ����ش����
                            foreach (var history in historyList.OrderByDescending(h => h.Timestamp))
                            {
                                HistoryList.Add(history);
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false; // ��ش�ʴ� ActivityIndicator
            }
        }

        // �ѧ��ѹ����Ѻ�ѹ�֡�����Ż���ѵԡ���ŧ
        public void SaveConversionHistory(double inputValue, string fromUnit, string toUnit, double resultValue)
        {
            var history = new ConversionHistory
            {
                InputValue = inputValue,
                FromUnit = fromUnit,
                ToUnit = toUnit,
                ResultValue = resultValue,
                Timestamp = DateTime.Now
            };

            // �ѹ�֡�����Ż���ѵԡ���ŧŧ����
            _conversionService.SaveHistoryToFile(history);

            // ��������ѵԡ���ŧ� ObservableCollection ��������ѻവ UI
            HistoryList.Insert(0, history);
        }

        // �ѧ��ѹ���������ѵԡ���ŧ
        private async void OnClearHistoryClicked(object sender, EventArgs e)
        {
            if (sender is Image clearButton)
            {
                // ��͢�Ҵ���������Ŵ��������ʢͧ˹�Ҩ;�����ѹ
                await Task.WhenAll(
                    clearButton.ScaleTo(0.8, 150, Easing.CubicIn) // ��͢�Ҵ����
                );

                // ������������ʡ�Ѻ��Т��»���
                await Task.WhenAll(
                    clearButton.ScaleTo(1.1, 150, Easing.BounceOut) // ���¢�Ҵ����
                );

                // ���絢�Ҵ������Ѻ�繻���
                await clearButton.ScaleTo(1, 100, Easing.BounceOut); // ���絢�Ҵ

                // ��ѧ�ҡ Animation �������� �ʴ���ͻ�Ѿ�׹�ѹ���ź
                bool isConfirmed = await DisplayAlert("Confirm Deletion", "Are you sure you want to clear the history?", "Yes, delete", "Cancel");

                if (isConfirmed)
                {
                    // ����������Ũҡ ObservableCollection
                    HistoryList.Clear();

                    // ź������纻���ѵԡ���ŧ
                    string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "conversion_history.json");
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
            }
        }


    }
}
