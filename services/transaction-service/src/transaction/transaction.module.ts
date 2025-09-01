import { forwardRef, Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';

import { TransactionController } from './controllers/transaction.controller';
import { TransactionService } from './services/transaction.service';
import { Transaction } from './entities/transaction.entity';
import { KafkaModule } from 'src/kafka/kafka.module';

@Module({
  imports: [
    TypeOrmModule.forFeature([Transaction]),
    forwardRef(() => KafkaModule),
  ],
  controllers: [TransactionController],
  providers: [TransactionService],
  exports: [TransactionService],
})
export class TransactionModule {}
