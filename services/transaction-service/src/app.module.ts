import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { ConfigModule, ConfigService } from '@nestjs/config';

import { TransactionModule } from './transaction/transaction.module';
import { KafkaModule } from './kafka/kafka.module';
import { Transaction } from './transaction/entities/transaction.entity';

@Module({
  imports: [
    ConfigModule.forRoot({
      isGlobal: true,
    }),
    TypeOrmModule.forRootAsync({
      imports: [ConfigModule],
      useFactory: (configService: ConfigService) => ({
        type: 'postgres',
        url: configService.get<string>('DATABASE_URL'),
        entities: [Transaction],
        synchronize: true, // Only for development
        logging: true,
      }),
      inject: [ConfigService],
    }),
    TransactionModule,
    KafkaModule,
  ],
})
export class AppModule {}
