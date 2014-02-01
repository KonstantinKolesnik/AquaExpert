#ifndef OPERATION_H_
#define OPERATION_H_

void AddToOperationQueue(Message_t *msg);
Message_t* GetFromOperationQueue();

#endif /* OPERATION_H_ */