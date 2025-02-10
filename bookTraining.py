import requests
import json

def bookTraining(id, token, memberId):

    url = "https://crossfit-lea.cms.efitness.com.pl/Schedule/RegisterForClass"

    payload=f"id={id}&memberID={memberId}&promoCodeID=&promoCode="
    headers = {
    'User-Agent': 'Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:97.0) Gecko/20100101 Firefox/97.0',
    'Accept': '*/*',
    'Accept-Language': 'en-US,en;q=0.5',
    'Accept-Encoding': 'gzip, deflate, br',
    'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8',
    'X-Requested-With': 'XMLHttpRequest',
    'Origin': 'https://crossfit-lea.cms.efitness.com.pl',
    'DNT': '1',
    'Connection': 'keep-alive',
    'Referer': 'https://crossfit-lea.cms.efitness.com.pl/kalendarz-zajec',
    'Cookie': f'.ASPXAUTH_Cms={token};',
    'Sec-Fetch-Dest': 'empty',
    'Sec-Fetch-Mode': 'cors',
    'Sec-Fetch-Site': 'same-origin'
    }

    response = requests.request("POST", url, headers=headers, data=payload)
    print(response.text)
    try:
        result = json.loads(response.text)['Success']
    except Exception:
        print('/n Booking failed')
        raise
    return result
