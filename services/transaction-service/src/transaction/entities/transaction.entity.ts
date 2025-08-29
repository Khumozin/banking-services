import {
  Entity,
  PrimaryGeneratedColumn,
  Column,
  CreateDateColumn,
  UpdateDateColumn,
} from 'typeorm';

@Entity('transactions')
export class Transaction {
  @PrimaryGeneratedColumn('uuid', { name: 'transaction_id' })
  transactionId: string;

  @Column({ name: 'source_account_id', type: 'uuid', nullable: true })
  sourceAccountId?: string;

  @Column({ name: 'destination_account_id', type: 'uuid', nullable: true })
  destinationAccountId?: string;

  @Column({ type: 'decimal', precision: 18, scale: 2 })
  amount: number;

  @Column({ length: 20, default: 'PENDING' })
  status: string;

  @Column({ name: 'transaction_type', length: 20 })
  transactionType: string;

  @CreateDateColumn({ name: 'created_at' })
  createdAt: Date;

  @UpdateDateColumn({ name: 'updated_at' })
  updatedAt: Date;
}
