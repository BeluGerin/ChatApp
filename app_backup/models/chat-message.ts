export interface ChatMessage {
  user: string;
  text: string;
  sentAt: string; // ISO string dari DateTime C#
}

export type ConnectionStatus =
  | 'disconnected'
  | 'connecting'
  | 'connected'
  | 'reconnecting';
