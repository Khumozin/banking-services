import {
  forwardRef,
  Inject,
  Injectable,
  Logger,
  OnModuleDestroy,
  OnModuleInit,
} from '@nestjs/common';
import { ConfigService } from '@nestjs/config';
import { Kafka, Producer, Consumer } from 'kafkajs';

import { TransactionResultEvent } from '../../transaction/dtos/transaction.dto';
import { TransactionService } from '../../transaction/services/transaction.service';

@Injectable()
export class KafkaService implements OnModuleInit, OnModuleDestroy {
  private readonly logger = new Logger(KafkaService.name);
  private kafka: Kafka;
  private producer: Producer;
  private consumer: Consumer;

  constructor(
    private readonly configService: ConfigService,
    @Inject(forwardRef(() => TransactionService))
    private readonly transactionService: TransactionService,
  ) {
    this.kafka = new Kafka({
      clientId: 'transaction-service',
      brokers: this.configService
        .get<string>('KAFKA_BROKERS', 'localhost:9092')
        .split(','),
    });

    this.producer = this.kafka.producer();
    this.consumer = this.kafka.consumer({
      groupId: 'transaction-service-group',
    });
  }

  async onModuleInit() {
    try {
      await this.producer.connect();
      await this.consumer.connect();

      // Subscribe to transaction result events
      await this.consumer.subscribe({
        topics: ['transaction.completed', 'transaction.failed'],
      });

      await this.consumer.run({
        eachMessage: async ({ topic, message }) => {
          try {
            const eventData = JSON.parse(
              message.value?.toString() || '{}',
            ) as TransactionResultEvent;
            this.logger.log(`Received event from topic ${topic}:`, eventData);

            if (topic === 'transaction.completed') {
              await this.transactionService.updateTransactionStatus(
                eventData.transactionId,
                'COMPLETED',
              );
            } else if (topic === 'transaction.failed') {
              await this.transactionService.updateTransactionStatus(
                eventData.transactionId,
                'FAILED',
                eventData.reason,
              );
            }
          } catch (error) {
            this.logger.error(
              `Error processing message from topic ${topic}:`,
              error,
            );
          }
        },
      });

      this.logger.log('Kafka service initialized successfully');
    } catch (error) {
      this.logger.error('Failed to initialize Kafka service:', error);
    }
  }

  async onModuleDestroy() {
    try {
      await this.consumer.disconnect();
      await this.producer.disconnect();
      this.logger.log('Kafka service disconnected');
    } catch (error) {
      this.logger.error('Error disconnecting Kafka service:', error);
    }
  }

  async publish(topic: string, message: any): Promise<void> {
    try {
      await this.producer.send({
        topic,
        messages: [
          {
            key: Date.now().toString(),
            value: JSON.stringify(message),
          },
        ],
      });

      this.logger.log(`Message published to topic ${topic}:`, message);
    } catch (error) {
      this.logger.error(`Failed to publish message to topic ${topic}:`, error);
      throw error;
    }
  }
}
