import sys
import pandas as pd
import json

dataframe = pd.read_excel("Resources/ExcelFiles/RAPOR2025.xlsx")

dataframe["Giriş Tarihi"] = pd.to_datetime(dataframe["Giriş Tarihi"], dayfirst=True)
dataframe["Çıkış Tarihi"] = pd.to_datetime(dataframe["Çıkış Tarihi"], dayfirst=True)

def get_data_by_dates(dates_string):
    sys.stdout.reconfigure(encoding='utf-8')
    result = ""

    for date_string in dates_string:
        target_date = pd.to_datetime(date_string)

        filtered = dataframe[dataframe["Giriş Tarihi"].dt.date == target_date.date()]

        result += "{\n"
        result += f'"Tarih": "{target_date.strftime("%Y-%m-%d")}",\n'
        result += '"Veriler": [\n'
        result += filtered.to_string(index=False)
        result += "\n],\n"
        result += '"Günün özeti": ' + target_date.strftime("%Y-%m-%d") + ',\n'
        result += "},\n"



    finalResult = result[:-2]
    print(finalResult)


def get_first_and_last_date():
    sys.stdout.reconfigure(encoding='utf-8')

    first_date = dataframe["Giriş Tarihi"].min().strftime("%Y-%m-%d")
    last_date = dataframe["Giriş Tarihi"].max().strftime("%Y-%m-%d")

    print(f"{first_date},{last_date}")

def get_last_day():
    sys.stdout.reconfigure(encoding='utf-8')

    last_date = dataframe["Giriş Tarihi"].max().strftime("%Y-%m-%d")
    target_date = pd.to_datetime(last_date)

    filtered = dataframe[dataframe["Giriş Tarihi"].dt.date == target_date.date()]


    result = ""
    result += "{\n"
    result += filtered.to_string(index=False)
    result += "\n}"

    print(result)


if __name__ == "__main__":
    if (sys.argv[1] == "get_data_by_dates"):
        get_data_by_dates(sys.argv[2:])
        
    elif(sys.argv[1] == "get_first_and_last_date"):
        get_first_and_last_date()

    elif(sys.argv[1] == "get_last_day"):
        get_last_day()