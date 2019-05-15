@echo off
setlocal enabledelayedexpansion
for /f "delims=: tokens=2" %%n in ('netsh wlan show interface name="Wi-Fi" ^| findstr "Profile"') do set Network=%%n
set name=!Network: =%! 
IF %name% == <%NetworkName%> (
 cmd /k "git config --global https.proxy "<%ProxyUrl%>" & git config --global http.proxy "<%ProxyUrl%>""
 ) ELSE (
		cmd /k "git config --global --unset http.proxy & git config --global --unset https.proxy"
 )

