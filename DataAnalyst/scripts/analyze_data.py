import pandas as pd
import matplotlib.pyplot as plt

def analyze_data(file_path):
    # CSV dosyasını oku
    data = pd.read_csv(file_path, delimiter=';', encoding='utf-8-sig')

    # Veriyi işle, sütunları düzenle, grafik oluştur vb.

    # Örnek olarak sadece DurationTime sütununun histogramını çıkaralım:
    data['DurationTime'] = data['DurationTime'].str.replace(',', '.').astype(float)
    fig, ax = plt.subplots(figsize=(10, 6))
    data['DurationTime'].hist(bins=30, ax=ax)
    ax.set_title('Distribution of DurationTime')
    ax.set_xlabel('DurationTime')
    ax.set_ylabel('Frequency')

    # Grafiği kaydetme
    output_path = file_path.replace('.csv', '.png')
    plt.savefig(output_path)
    plt.close()  # Grafik kaynağı sıfırla

    return output_path  # Grafik dosyasının yolunu döndür

if __name__ == "__main__":
    import sys
    analyze_data(sys.argv[1])
