import { Logger, Module, OnModuleDestroy, OnModuleInit } from '@nestjs/common';
import { NotificationService } from './services/notification.service';
import { NotificationController } from './controllers/notification.controller';
import { Kafka, Consumer } from 'kafkajs';
import { ConfigService } from '@nestjs/config';

interface TransactionResultEvent {
  transactionId: string;
  status: 'COMPLETED' | 'FAILED';
  reason?: string;
}

@Module({
  controllers: [NotificationController],
  providers: [NotificationService],
})
export class NotificationModule implements OnModuleInit, OnModuleDestroy {
  private readonly logger = new Logger(NotificationService.name);
  private kafka: Kafka;
  private consumer: Consumer;

  constructor(private readonly configService: ConfigService) {
    this.kafka = new Kafka({
      clientId: 'notification-service',
      brokers: this.configService
        .get<string>('KAFKA_BROKERS', 'localhost:9092')
        .split(','),
    });

    this.consumer = this.kafka.consumer({
      groupId: 'notification-service-group',
    });
  }

  async onModuleInit() {
    try {
      await this.consumer.connect();

      // Subscribe to transaction result events
      await this.consumer.subscribe({
        topics: ['transaction.completed', 'transaction.failed'],
      });

      await this.consumer.run({
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
        eachMessage: async ({ topic, partition, message }) => {
          try {
            const eventData = JSON.parse(
              message.value?.toString() || '{}',
            ) as TransactionResultEvent;
            this.logger.log(`Received event from topic ${topic}:`, eventData);

            if (topic === 'transaction.completed') {
              await this.sendTransactionCompletedNotification(eventData);
            } else if (topic === 'transaction.failed') {
              await this.sendTransactionFailedNotification(eventData);
            }
          } catch (error) {
            this.logger.error(
              `Error processing message from topic ${topic}:`,
              error,
            );
          }
        },
      });

      this.logger.log(
        'Notification service initialized and listening for events',
      );
    } catch (error) {
      this.logger.error('Failed to initialize Notification service:', error);
    }
  }

  async onModuleDestroy() {
    try {
      await this.consumer.disconnect();
      this.logger.log('Notification service disconnected');
    } catch (error) {
      this.logger.error('Error disconnecting Notification service:', error);
    }
  }

  private async sendTransactionCompletedNotification(
    event: TransactionResultEvent,
  ): Promise<void> {
    // Mock notification - in a real system, this would send email/SMS
    const notification = {
      type: 'TRANSACTION_COMPLETED',
      transactionId: event.transactionId,
      status: event.status,
      timestamp: new Date().toISOString(),
      message: `Transaction ${event.transactionId} has been completed successfully.`,
    };

    // Log notification (mock email/SMS)
    this.logger.log('📧 EMAIL NOTIFICATION SENT:');
    this.logger.log('================================');
    this.logger.log(`Subject: Transaction Completed - ${event.transactionId}`);
    this.logger.log(`Dear Customer,`);
    this.logger.log(
      `Your transaction (ID: ${event.transactionId}) has been processed successfully.`,
    );
    this.logger.log(`Status: ${event.status}`);
    this.logger.log(`Time: ${notification.timestamp}`);
    this.logger.log(`Thank you for using our banking services.`);
    this.logger.log('================================');

    this.logger.log('📱 SMS NOTIFICATION SENT:');
    this.logger.log('================================');
    this.logger.log(
      `Banking Alert: Transaction ${event.transactionId} completed successfully. Status: ${event.status}. Time: ${notification.timestamp}`,
    );
    this.logger.log('================================');
  }

  private async sendTransactionFailedNotification(
    event: TransactionResultEvent,
  ): Promise<void> {
    // Mock notification - in a real system, this would send email/SMS
    const notification = {
      type: 'TRANSACTION_FAILED',
      transactionId: event.transactionId,
      status: event.status,
      reason: event.reason || 'Unknown error',
      timestamp: new Date().toISOString(),
      message: `Transaction ${event.transactionId} has failed.`,
    };

    // Log notification (mock email/SMS)
    this.logger.log('📧 EMAIL NOTIFICATION SENT:');
    this.logger.log('================================');
    this.logger.log(`Subject: Transaction Failed - ${event.transactionId}`);
    this.logger.log(`Dear Customer,`);
    this.logger.log(
      `Unfortunately, your transaction (ID: ${event.transactionId}) could not be processed.`,
    );
    this.logger.log(`Status: ${event.status}`);
    this.logger.log(`Reason: ${notification.reason}`);
    this.logger.log(`Time: ${notification.timestamp}`);
    this.logger.log(`Please contact customer support if you need assistance.`);
    this.logger.log('================================');

    this.logger.log('📱 SMS NOTIFICATION SENT:');
    this.logger.log('================================');
    this.logger.log(
      `Banking Alert: Transaction ${event.transactionId} failed.`,
    );
    this.logger.log(`Reason: ${notification.reason}.`);
    this.logger.log(`Time: ${notification.timestamp}.`);
    this.logger.log(`Contact support if needed.`);
    this.logger.log('================================');
  }

  // Method to send custom notifications (for future use)
  sendCustomNotification(
    type: string,
    message: string,
    recipient?: string,
  ): void {
    const notification = {
      type,
      message,
      recipient,
      timestamp: new Date().toISOString(),
    };

    this.logger.log('📧 CUSTOM NOTIFICATION SENT:');
    this.logger.log('================================');
    this.logger.log(`Type: ${type}`);
    this.logger.log(`Message: ${message}`);
    this.logger.log(`Recipient: ${recipient || 'System Default'}`);
    this.logger.log(`Time: ${notification.timestamp}`);
    this.logger.log('================================');
  }
}
