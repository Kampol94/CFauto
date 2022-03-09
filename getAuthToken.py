from signal import raise_signal
import requests
import urllib.parse

def getAuthToken(login, password):
    url = "https://centrumfitnesslea-krakow.cms.efitness.com.pl/Login/SystemLogin?returnUrl=https%3A%2F%2Fcentrumfitnesslea-krakow.cms.efitness.com.pl%2F"

    payload=f'Login={urllib.parse.quote(login)}&Password={password}&Direct=&SubmitCredentials=Zaloguj%2Bsi%C4%99'
    headers = {
    'User-Agent': 'Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:97.0) Gecko/20100101 Firefox/97.0',
    'Accept': 'text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8',
    'Accept-Language': 'en-US,en;q=0.5',
    'Accept-Encoding': 'gzip, deflate, br',
    'Content-Type': 'application/x-www-form-urlencoded',
    'Origin': 'https://centrumfitnesslea-krakow.cms.efitness.com.pl',
    'DNT': '1',
    'Connection': 'keep-alive',
    'Referer': 'https://centrumfitnesslea-krakow.cms.efitness.com.pl/Login/SystemLogin?returnurl=https://centrumfitnesslea-krakow.cms.efitness.com.pl/',
    'Upgrade-Insecure-Requests': '1',
    'Sec-Fetch-Dest': 'document',
    'Sec-Fetch-Mode': 'navigate',
    'Sec-Fetch-Site': 'same-origin',
    'Sec-Fetch-User': '?1'
    }
    s = requests.Session() 
    r = s.post(url, headers=headers, data=payload)
    if r.status_code != 200:
        print(r.status_code)
        print(r.text)
        raise
    try:
        token = s.cookies['.ASPXAUTH_Cms']
    except Exception:
        print('Getting token failed')
        raise

    return token