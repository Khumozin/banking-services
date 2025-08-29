import { Module } from '@nestjs/common';
import { AppController } from './app.controller';
import { AppService } from './app.service';
import { TransactionModule } from './transaction/transaction.module';
import { KafkaModule } from './kafka/kafka.module';

@Module({
  imports: [TransactionModule, KafkaModule],
  controllers: [AppController],
  providers: [AppService],
})
export class AppModule {}
