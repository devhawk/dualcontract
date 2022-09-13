using System;
using System.ComponentModel;
using Neo;
using Neo.SmartContract;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace contract2;
public class TestContract2 : SmartContract
{
    [InitialValue("NNVaYFKxp7eacyY2uPLz5oZXwJUN5FhDfd", ContractParameterType.Hash160)]
    static readonly UInt160 Alice = default!;

    [InitialValue("NPybbE7BdYn9RAuRy6K168sxcGeYmjnseq", ContractParameterType.Hash160)]
    static readonly UInt160 Bob = default!;

    [InitialValue("Nct3yaEtgLfSdzKd7SLwE2q17f6byeeqYo", ContractParameterType.Hash160)]
    static readonly UInt160 Charlie = default!;

    public static void TestInvoke()
    {
        var aliceBalance = NEO.BalanceOf(Alice);
        var bobBalance = NEO.BalanceOf(Bob);
        var charlieBalance = NEO.BalanceOf(Charlie);

        var result = NEO.Transfer(Alice, Charlie, 100);

        var aliceBalance2 = NEO.BalanceOf(Alice);
        var bobBalance2 = NEO.BalanceOf(Bob);
        var charlieBalance3 = NEO.BalanceOf(Charlie);
    }

    const byte Prefix_ContractOwner = 0xFF;

    [DisplayName("_deploy")]
    public void Deploy(object data, bool update)
    {
        if (update) return;

        var tx = (Transaction)Runtime.ScriptContainer;
        var key = new byte[] { Prefix_ContractOwner };
        Storage.Put(Storage.CurrentContext, key, tx.Sender);
    }

    public static void Update(ByteString nefFile, string manifest)
    {
        var key = new byte[] { Prefix_ContractOwner };
        var contractOwner = (UInt160)Storage.Get(Storage.CurrentContext, key);
        var tx = (Transaction)Runtime.ScriptContainer;

        if (!contractOwner.Equals(tx.Sender))
        {
            throw new Exception("Only the contract owner can update the contract");
        }

        ContractManagement.Update(nefFile, manifest, null);
    }
}
