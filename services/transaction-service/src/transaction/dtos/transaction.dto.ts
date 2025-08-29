import { IsNotEmpty, IsUUID, IsNumber, IsPositive } from 'class-validator';
import { ApiProperty } from '@nestjs/swagger';

export class DepositDto {
  @ApiProperty({
    description: 'Destination account ID',
    example: '123e4567-e89b-12d3-a456-426614174000',
  })
  @IsNotEmpty()
  @IsUUID()
  destinationAccountId: string;

  @ApiProperty({ description: 'Amount to deposit', example: 100.5 })
  @IsNotEmpty()
  @IsNumber()
  @IsPositive()
  amount: number;
}

export class TransferDto {
  @ApiProperty({
    description: 'Source account ID',
    example: '123e4567-e89b-12d3-a456-426614174000',
  })
  @IsNotEmpty()
  @IsUUID()
  sourceAccountId: string;

  @ApiProperty({
    description: 'Destination account ID',
    example: '987fcdeb-51a2-43d1-9f8e-123456789abc',
  })
  @IsNotEmpty()
  @IsUUID()
  destinationAccountId: string;

  @ApiProperty({ description: 'Amount to transfer', example: 50.25 })
  @IsNotEmpty()
  @IsNumber()
  @IsPositive()
  amount: number;
}

export class TransactionResponseDto {
  @ApiProperty({ description: 'Transaction ID' })
  transactionId: string;

  @ApiProperty({ description: 'Source account ID', required: false })
  sourceAccountId?: string;

  @ApiProperty({ description: 'Destination account ID', required: false })
  destinationAccountId?: string;

  @ApiProperty({ description: 'Transaction amount' })
  amount: number;

  @ApiProperty({ description: 'Transaction status' })
  status: string;

  @ApiProperty({ description: 'Transaction type' })
  transactionType: string;

  @ApiProperty({ description: 'Creation timestamp' })
  createdAt: Date;

  @ApiProperty({ description: 'Last update timestamp' })
  updatedAt: Date;
}

// Event DTOs for Kafka
export interface TransactionInitiatedEvent {
  transactionId: string;
  sourceAccountId?: string;
  destinationAccountId?: string;
  amount: number;
  transactionType: string;
}

export interface TransactionResultEvent {
  transactionId: string;
  status: 'COMPLETED' | 'FAILED';
  reason?: string;
}
