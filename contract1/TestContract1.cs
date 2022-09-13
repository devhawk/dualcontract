using System;
using System.ComponentModel;
using Neo;
using Neo.SmartContract;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace contract1;

[ContractPermission("*", "testInvoke")]
public class TestContract1 : SmartContract
{
    const byte Prefix_ContractOwner = 0xFF;

    [InitialValue("NNVaYFKxp7eacyY2uPLz5oZXwJUN5FhDfd", ContractParameterType.Hash160)]
    static readonly UInt160 Alice = default!;

    [InitialValue("NPybbE7BdYn9RAuRy6K168sxcGeYmjnseq", ContractParameterType.Hash160)]
    static readonly UInt160 Bob = default!;

    [InitialValue("Nct3yaEtgLfSdzKd7SLwE2q17f6byeeqYo", ContractParameterType.Hash160)]
    static readonly UInt160 Charlie = default!;

    [InitialValue("0x5af85af52d34e75c75399562da81dc722832961f", ContractParameterType.Hash160)]
    static readonly UInt160 TestContract2 = default!;

    static void PrintBalances(string msg)
    {
        var aliceBalance = NEO.BalanceOf(Alice);
        var bobBalance = NEO.BalanceOf(Bob);
        var charlieBalance = NEO.BalanceOf(Charlie);
        Runtime.Log($"{msg}: A:{aliceBalance} B:{bobBalance} C:{charlieBalance}");
    }

    public static void TestInvoke()
    {
        PrintBalances("Before Transfer");
        var result = NEO.Transfer(Alice, Bob, 100);
        PrintBalances("After Transfer, Before Call");
        Contract.Call(TestContract2, "testInvoke", CallFlags.All);
        PrintBalances("After Call");
    }

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
