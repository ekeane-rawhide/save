import * as signalR from '@microsoft/signalr'

let connection: signalR.HubConnection | null = null

export function getHubConnection(getToken: () => string | null): signalR.HubConnection {
  if (connection) return connection

  connection = new signalR.HubConnectionBuilder()
    .withUrl(import.meta.env.VITE_HUB_URL, {
      accessTokenFactory: () => getToken() ?? '',
    })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Warning)
    .build()

  return connection
}

export async function stopHubConnection() {
  if (!connection) return
  await connection.stop()
  connection = null
}
